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
        var i = 0;
        //Console.WriteLine("Length");
        writer.Write((int)ExpirationDate.Year);
        //Console.WriteLine(i++.ToString()+" "+writer.BaseStream.Length);
        writer.Write((short)ExpirationDate.Month);
        //Console.WriteLine(i++.ToString()+" "+writer.BaseStream.Length);
        writer.Write((short)ExpirationDate.Day);
        //Console.WriteLine(i++.ToString()+" "+writer.BaseStream.Length);
        writer.Write((short)FeatureAccess);
        //Console.WriteLine(i++.ToString()+" "+writer.BaseStream.Length);
        writer.Write((int)Version.Major);
        //Console.WriteLine(i++.ToString()+" "+writer.BaseStream.Length);
        writer.Write((int)Version.Minor);
        //Console.WriteLine(i++.ToString()+" "+writer.BaseStream.Length);
        writer.Write((string)Program.ToString());
        //Console.WriteLine(i++.ToString()+" "+writer.BaseStream.Length);
        writer.Write((string)Name);
        //Console.WriteLine(i++.ToString()+" "+writer.BaseStream.Length);
        writer.Write((string)ContactInfo);
        //Console.WriteLine(i++.ToString()+" "+writer.BaseStream.Length);

        // Write an array of Keys
        writer.Write((short)Keys.Length);
        foreach (var key in Keys)
        {
            writer.Write(key);
        }

    }

    public void ReadFromStream(BinaryReader r)
    {
        var i = 0;
        //Console.WriteLine($"Length: {r.BaseStream.Length}");
        var t1 = r.ReadInt32();
        //Console.WriteLine(i++.ToString()+" "+r.BaseStream.Position);
        var t2 = r.ReadInt16();
        //Console.WriteLine(i++.ToString()+" "+r.BaseStream.Position);
        var t3 = r.ReadInt16();
        //Console.WriteLine(i++.ToString()+" "+r.BaseStream.Position);
        ExpirationDate = new DateOnly(t1,t2, t3);
        FeatureAccess = r.ReadInt16();
        //Console.WriteLine(i++.ToString()+" "+r.BaseStream.Position);
        var major = r.ReadInt32();
        //Console.WriteLine(i++.ToString()+" "+r.BaseStream.Position);
        var minor = r.ReadInt32();
        //Console.WriteLine(i++.ToString()+" "+r.BaseStream.Position);
        Version = new Version(major, minor);
        Program = r.ReadString();
        //Console.WriteLine(i++.ToString()+" "+r.BaseStream.Position);
        Name = r.ReadString();
        //Console.WriteLine(i++.ToString()+" "+r.BaseStream.Position);
        ContactInfo = r.ReadString();
        //Console.WriteLine(i++.ToString()+" "+r.BaseStream.Position);
        
        // Read an array of Keys
        var keyCount = r.ReadInt16();
        Keys = new string[keyCount];
    }
}