using System.Text.Json.Serialization;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;

namespace Fileio;

public enum ExpirationUnit
{
    seconds,
    minutes,
    hours,
    days,
    weeks,
    Months,
    Quarters,
    years
}

public class FileClient(string apiKey) : RestClient(new RestClientOptions("https://file.io")
        {
            Authenticator = new JwtAuthenticator(apiKey),
        }, configureSerialization: s => s.UseSystemTextJson(new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        }))
{
    public async Task<FileDetailsResponse> UploadFile(Stream file, string fileName, int expiresIn = 14, ExpirationUnit unit = ExpirationUnit.days, int? maxDownloads = null, bool? autoDelete = null)
    {
        var request = new RestRequest("uploadFile", Method.Post);

        request.AddFile("file",() => file, fileName);

        request.AddParameter("expires", expiresIn + unit.ToString());

        if(maxDownloads.HasValue)
            request.AddParameter("maxDownloads", maxDownloads.ToString());

        if(autoDelete.HasValue)
            request.AddParameter("autoDelete", autoDelete.ToString());

        // Execute the request
        var response = await this.ExecuteAsync<FileDetailsResponse>(request);
        response.ThrowIfError();

        return response.Data!;
    }

    public async Task<List<FileDetails>?> GetFiles(string? search = null, string? sort = null, int offset = 0, int limit = 10)
    {
        var request = new RestRequest("/", Method.Get);

        // Add query parameters
        if(!string.IsNullOrEmpty(search))
            request.AddParameter("search", search);

        if(!string.IsNullOrEmpty(sort))
            request.AddParameter("sort", sort);

        request.AddParameter("offset", offset);
        request.AddParameter("limit", limit);

        // Execute the request
        var response = await this.ExecuteAsync<ListResponse<FileDetails>>(request);
        response.ThrowIfError();

        return response.Data?.Nodes;
    }

    public async Task<Stream?> DownloadFile(string  key)
    {
        var request = new RestRequest("/{key}", Method.Get);
        request.AddUrlSegment("key", key);

        // Execute the request
        var response = await this.ExecuteAsync(request);
        response.ThrowIfError();

        // Return the file data
        return response.RawBytes != null ? new MemoryStream(response.RawBytes) : null;
    }

    public async Task<FileDetails?> UpdateFile(string key, Stream? file = null, int? expiresIn = null, ExpirationUnit? unit = null, int? maxDownloads = null, bool? autoDelete = null)
    {
        var request = new RestRequest("/{key}", Method.Put);
        request.AddUrlSegment("key", key);

        // Add request body parameters
        if(file != null)
            request.AddFile("file", () => file, "filename");
        if(expiresIn.HasValue)
            request.AddParameter("expires", $"{expiresIn.Value} {unit}");
        if(maxDownloads.HasValue)
            request.AddParameter("maxDownloads", maxDownloads.Value);
        if(autoDelete.HasValue)
            request.AddParameter("autoDelete", autoDelete.Value);

        // Execute the request
        var response = await this.ExecuteAsync<FileDetails>(request);
        response.ThrowIfError();

        return response.Data;
    }

    public async Task<Response> DeleteFile(string key)
    {
        var request = new RestRequest("/{key}", Method.Delete);
        request.AddUrlSegment("key", key);

        // Execute the request
        var response = await this.ExecuteAsync<Response>(request);
        response.ThrowIfError();

        return response.Data!;
    }

}
