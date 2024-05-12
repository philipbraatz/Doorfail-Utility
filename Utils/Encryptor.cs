using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Doorfail.Core.Util;
public static class Encryptor
{
    public static byte[] Encrypt<T>(T plainText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if(plainText == null)
            throw new ArgumentNullException(nameof(plainText));
        if(Key?.Any() != true)
            throw new ArgumentNullException(nameof(Key));
        if(IV?.Any() != true)
            throw new ArgumentNullException(nameof(IV));

        using var aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new();
        using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);

        switch(plainText)
        {
            case string s:
                byte[] stringBytes = Encoding.UTF8.GetBytes(s);
                csEncrypt.Write(stringBytes, 0, stringBytes.Length);
                break;
            case byte[] b:
                csEncrypt.Write(b, 0, b.Length);
                break;
            default:
                string jsonString = JsonSerializer.Serialize(plainText);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                csEncrypt.Write(jsonBytes, 0, jsonBytes.Length);
                break;
        }

        csEncrypt.FlushFinalBlock(); // Ensure all data is flushed to the underlying stream

        return msEncrypt.ToArray();
    }

    private static MemoryStream Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if(cipherText == null || cipherText.Length == 0)
            throw new ArgumentNullException(nameof(cipherText));
        if(Key == null || Key.Length == 0)
            throw new ArgumentNullException(nameof(Key));
        if(IV == null || IV.Length == 0)
            throw new ArgumentNullException(nameof(IV));

        using var aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(cipherText);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var msPlainText = new MemoryStream();

        // Read the decrypted data from the CryptoStream into the MemoryStream
        csDecrypt.CopyTo(msPlainText);

        // Reset the position of the MemoryStream to the beginning
        msPlainText.Position = 0;

        // Return the MemoryStream containing the decrypted data
        return msPlainText;
    }

    public static string Decrypt_AsString(byte[] cipherText, byte[] key, byte[] IV)
        => Encoding.UTF8.GetString(Decrypt(cipherText, key, IV).ToArray());

    public static byte[] Decrypt_AsByte(byte[] cipherText, byte[] key, byte[] IV)
        => Decrypt(cipherText, key, IV).ToArray();
}
