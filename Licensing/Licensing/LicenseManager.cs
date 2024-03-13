﻿using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Doorfail.Core.Logging;
using Doorfail.Core.Models;
using Doorfail.Core.Util;
using Fileio;

namespace Doorfail.Core.Licensing;

public class LicenseManager(string fileioKey, string programName)
{
    private const string encryptionKey = "rDFXWy88Dsi7qHRjNvlHBSCVZQFOXeU/5CTqTAqR+sY=";
    public byte[] InitializationVector { get; set; } = [
        (byte)(Environment.Version.Major % 256),
        (byte)(Environment.Version.Minor % 256),
        209,
        15,
        13,
        108,
        251,
        89,
        168,
        189,
        218,
        119,
        187,
        88,
        184,
        209];
    private readonly string programName = programName;
    private readonly string MasterApiKey = fileioKey;
    public License License { get; private set; }

#if DEBUG

    public License GenerateLicenseKey()
    {
        License license = new()
        {
            Program = programName,
            Version = Environment.Version
        };
        var rawData = JsonSerializer.Serialize(license);

        using var aesAlg = Aes.Create();
        aesAlg.Key = Convert.FromBase64String(encryptionKey);
        aesAlg.IV = InitializationVector;

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        var encryptedData = encryptor.TransformFinalBlock(Convert.FromBase64String(rawData), 0, rawData.Length);

        license.Id = Convert.ToBase64String(encryptedData);

        return license;
    }

    public License GenerateLicenseKey(License license)
    {
        var rawData =  Entity<string>.Compress(license);
        var key = Convert.FromBase64String(encryptionKey);
        var encryptedData = Encryptor.Encrypt(rawData, key, InitializationVector);

        license.Program = programName;
        license.Id = Convert.ToBase64String(encryptedData);
        license.Version = Environment.Version;

        return license;
    }

    public static byte[] GenerateRandomInitializationVector()
    {
        var iv = new byte[16];
        using(var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }
        return iv;
    }

    public static byte[] GenerateAesKey(int keySize)
    {
        using Aes aes = Aes.Create();
        aes.KeySize = keySize;
        aes.GenerateKey();
        return aes.Key;
    }

#endif

    public License DecodeLicenseKey(string licenseKey)
    {
        if(string.IsNullOrEmpty(licenseKey))
            throw new ArgumentNullException(nameof(licenseKey));

        License license = new();
        try
        {
            var key = Convert.FromBase64String(encryptionKey);

            var compressedData = Encryptor.Decrypt_AsByte(Convert.FromBase64String(licenseKey), key, InitializationVector);

            license = Entity<string>.Decompress<License>(compressedData);
            license.Id = licenseKey;
        } catch(JsonException e)
        {
            throw new LicenseException(license, e);
        }

        try
        {
            TempLogManager<LicenseEvent> manager = TempLogManager<LicenseEvent>.Initialize(new FileClient(license.Keys[0])).Result;
            var lastLog = manager.ReadLastLog(license.ShortKey).Result;
            if(lastLog is not null)
            {
                lastLog.UsageCount++;
                lastLog.Message = "License key used.";
            }

            lastLog ??= new LicenseEvent
            {
                LogDate = DateTimeOffset.Now,
                Message = "License key used for the first time.",
                UsageCount = 0
            };

            manager.AddLog(licenseKey, lastLog).Wait();
        } catch(Exception e)
        { } // No one can hear you scream.

        if(license.ExpirationDate < DateTimeOffset.Now)
            throw new LicenseException(license);

        if(license.Program != programName)
            throw new LicenseException(license, programName);

        License = license;
        return License;
    }

    public byte[] EncryptData(string data, string licenseKey)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey+licenseKey);
        aesAlg.IV = InitializationVector; // Use the provided IV

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        return encryptor.TransformFinalBlock(Convert.FromBase64String(data), 0, data.Length);
    }

    public T DecryptData<T>(byte[] encryptedData, License license)
        where T : Entity<string>
    {
        var key =  MergeKeys(Convert.FromBase64String(encryptionKey), Convert.FromBase64String(license.Id));
        var compressedData = Encryptor.Decrypt_AsByte(encryptedData, key, InitializationVector);
        try
        {
            var data = Entity<string>.Decompress<T>(compressedData);

            try
            {
                TempLogManager<LicenseEvent> manager = TempLogManager<LicenseEvent>.Initialize(new FileClient(license.Keys[0])).Result;
                var lastLog = manager.ReadLastLog(license.ShortKey).Result;
                if(lastLog is not null)
                {
                    lastLog.UsageCount++;
                    lastLog.Message = $"Decrypting {nameof(T)} {data}";
                }

                lastLog ??= new LicenseEvent
                {
                    LogDate = DateTimeOffset.Now,
                    Message = $"First use. Decrypting {nameof(T)} {data}",
                    UsageCount = 0
                };

                manager.AddLog(license.ShortKey, lastLog).Wait();
            } catch(Exception e)
            { } // No one can hear you scream.

            return data;
        } catch(JsonException e)
        {
            throw new LicenseException(License, e);
        }
    }

    private static byte[] MergeKeys(byte[] key1, byte[] key2)
    {
        if(key1.Length != key2.Length)
            throw new ArgumentException("Byte arrays must be of the same length");

        var mergedKey = new byte[key1.Length];

        for(var i = 0; i < key1.Length; i++)
        {
            mergedKey[i] = (byte)(key1[i] ^ key2[i]);
        }

        return mergedKey;
    }

    public async Task SetUsagesRemaining(string licenseKey, int usagesRemaining)
    {
        try
        {
            await (await TempLogManager<string>.Initialize(new(MasterApiKey)))
                .AddLog($"{programName}-{licenseKey}", usagesRemaining.ToString());
        } catch(Exception ex)
        {
            Console.WriteLine($"Error setting usages remaining for license key {licenseKey}: {ex.Message}");
        }
    }
}
