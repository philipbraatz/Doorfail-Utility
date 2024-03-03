using Doorfail.Core.Models;
using Doorfail.Core.Util.Extensions;
using System.Text.Json.Serialization;

namespace Doorfail.Core.Licensing;
public class License :NamedEntityUpdatable<string>
{
    public DateTime ExpirationDate { get; set; }
    public int FeatureAccess { get; set; }
    public string ContactInfo { get; set; }
    public string Program { get; set; }
    public Version Version { get; set; }
    public string[] Keys { get; set; }

    [JsonIgnore]
    public string ShortKey => $"{Program.RemoveNonAlpha().ToUpper()}-{Id[..8]}-{Name.RemoveNonAlpha().ToLower()}";
    
    public License()
    {
        CreatedOn = DateTimeOffset.Now;
    }
    public override string ToString() => $"License for {Program} expires on {ExpirationDate:yyyy-MM-dd}. Contact {ContactInfo} for renewal.";
    public string StartupText() => $@"~ {Program} v{Version} ~
~ Created by {ContactInfo} ~";

    
}