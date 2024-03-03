using System.Net;
using System.Text.Json.Serialization;

namespace Fileio;
public class FileDetails
{
    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public string Link { get; set; }
    public DateTime Expires { get; set; }
    public string Expiry { get; set; }
    public int Downloads { get; set; }
    public int MaxDownloads { get; set; }
    public bool AutoDelete { get; set; }
    public int Size { get; set; }
    public string MimeType { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}

public class FileDetailsResponse : FileDetails
{
    [JsonIgnore]
    public Response Response { get; set; }

    [JsonPropertyName("success")]
    public bool Success => Response.Success;

    [JsonPropertyName("status")]
    public HttpStatusCode Status => Response.Status;
}
