using Doorfail.Core.Util.Extensions;
using System.Text.Json.Serialization;
using Doorfail.Core.Models;

namespace Doorfail.Core.Licensing;
public class License :NamedEntityUpdatable<string>
{
    public DateOnly ExpirationDate { get; set; }
    public int FeatureAccess { get; set; }
    public string ContactInfo { get; set; }
    public string Program { get; set; }
    public Version Version { get; set; }
    public string[] Keys { get; set; }

    [JsonIgnore]
    public string ShortKey => $"{Program.RemoveNonAlpha().ToUpper()}-{Id[..8]}-{Name.RemoveNonAlpha().ToLower()}";

    public override string ToString() => $"License for {Program} expires on {ExpirationDate:yyyy-MM-dd}. Contact {ContactInfo} for renewal.";
    public string StartupText() => $@"~ {Program} v{Version} ~
~ Created by {ContactInfo} ~";

    public void WriteToStream(BinaryWriter writer)
    {
        writer.Write(ExpirationDate.Year);
        writer.Write((short)ExpirationDate.Month);
        writer.Write((short)ExpirationDate.Day);
        writer.Write((short)FeatureAccess);

        var contactInfo =StringCompressor.Compress(ContactInfo);
        writer.Write((short)contactInfo.Length);
        //writer.Write(ContactInfo.ToCharArray().Select(s => (byte)s).ToArray());
        Console.WriteLine($"ContactInfo Compressed '{StringCompressor.Decompress(contactInfo)}'|"+Convert.ToBase64String(contactInfo) +"|"+ contactInfo.Length);
        writer.Write(contactInfo);

        var program =//StringCompressor.Compress(
                     Program;//);
        //writer.Write((short)program.Length);
        writer.Write(program);

        writer.Write(Version.ToString());
        writer.Write((short)Keys.Length);
        foreach(var key in Keys)
        {
            writer.Write(key);
        }

        byte[]? name =StringCompressor.Compress(Name);
        writer.Write(name.Length);
        writer.Write(name);

        byte[]? desc =StringCompressor.Compress(Description);
        writer.Write(desc.Length);
        writer.Write(desc);
    }

    public void ReadFromStream(BinaryReader r)
    {
        ExpirationDate = new DateOnly(r.ReadInt32(), r.ReadInt16(), r.ReadInt16());
        FeatureAccess = r.ReadInt16();

        var compressedInfo = r.ReadBytes(r.ReadInt16());
        Console.WriteLine($"ContactInfo Compressed '{StringCompressor.Decompress(compressedInfo)}'|"+Convert.ToBase64String(compressedInfo) +"|"+ compressedInfo.Length);
        ContactInfo = StringCompressor.Decompress(compressedInfo);
        //var compressedInfo = r.ReadBytes(r.ReadInt16());
        //ContactInfo = new string(compressedInfo.Select(s => (char)s).ToArray());

        //compressedInfo = r.ReadBytes(r.ReadInt16());
        Program = r.ReadString();//StringCompressor.Decompress(compressedInfo);

        Version = new Version(r.ReadString());
        Keys = new string[r.ReadInt16()];
        for(int i = 0; i < Keys.Length; i++)
        {
            Keys[i] = r.ReadString();
        }
        Name = StringCompressor.Decompress(r.ReadBytes(r.ReadInt32()));
        Description = StringCompressor.Decompress(r.ReadBytes(r.ReadInt32()));
    }
}