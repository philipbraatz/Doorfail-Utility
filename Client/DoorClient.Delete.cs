using System.Collections.Specialized;
using RestSharp;

namespace Doorfail.Core.Client;

public partial class DoorClient
{
    public async Task DeleteAsync(string route = default, NameValueCollection parameters = default, object body = null)
    {
        var r = BuildRequest(route, Method.Delete, parameters);
        if(body is not null)
        { r.AddJsonBody(body); }

        await HandleResponse(this.ExecuteAsync(r));
    }

    public async Task<T> DeleteAsync<T>(string route = default, NameValueCollection parameters = default, object body = null)
    {
        var r = BuildRequest(route, Method.Delete, parameters);
        if(body is not null)
        { r.AddJsonBody(body); }

        return await HandleResponse(this.ExecuteAsync<T>(r));
    }

    public async Task DeleteAsync(string route = default, object id = default)
    {
        var r = BuildRequest(route, Method.Delete, nameof(id), id);
        await HandleResponse(ExecuteAsync(r));
    }

    public async Task<T> DeleteAsync<T>(string route = default, object id = default)
    {
        var r = BuildRequest(route, Method.Delete, nameof(id), id);
        return await HandleResponse(this.ExecuteAsync<T>(r));
    }

    public Task DeleteManyAsync(string route = default, NameValueCollection parameters = default, object body = null)
        => DeleteAsync(route, parameters, body);

    public Task<T> DeleteManyAsync<T>(string route = default, NameValueCollection parameters = default, object body = null)
        => DeleteAsync<T>(route, parameters, body);
}