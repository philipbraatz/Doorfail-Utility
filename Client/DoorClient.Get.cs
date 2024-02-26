using System.Collections.Specialized;
using RestSharp;

namespace Doorfail.Core.Client;

public partial class DoorClient
{
    public bool Ping() => GetAsync<bool>("ping").Result;

    public T Get<T>(string route, string name, object? value)
    {
        var r = BuildRequest(route, Method.Get, name, value);
        return HandleResponse(this.ExecuteAsync<T>(r).Result);
    }

    public async Task<T> GetAsync<T>(string route = default, NameValueCollection parameters = default)
    {
        var r = BuildRequest(route, Method.Get, parameters);
        return HandleResponse(await this.ExecuteAsync<T>(r));
    }

    public async Task<T> GetAsync<T>(string route, string name, object? value)
    {
        var r = BuildRequest(route, Method.Get, name, value);
        return HandleResponse(await this.ExecuteAsync<T>(r));
    }

    public async Task GetAsync(string route = default, NameValueCollection parameters = default)
    {
        var r = BuildRequest(route, Method.Get, parameters);
        await HandleResponse(this.ExecuteAsync(r));
    }

    public async Task GetAsync(string route, string name, object? value)
    {
        var r = BuildRequest(route, Method.Get, name, value);
        await HandleResponse(this.ExecuteAsync(r));
    }

    public Task<IDictionary<K, V>> GetDictionaryAsync<K, V>(string route = default, NameValueCollection parameters = default)
        where K : notnull
        where V : notnull
    => GetAsync<IDictionary<K, V>>(route, parameters);

    public Task<IDictionary<K, V>> GetDictionaryAsync<K, V>(string route = default, string name = null, object value = null)
        where K : notnull
        where V : notnull
    => GetAsync<IDictionary<K, V>>(route, name, value);

    public Task<IList<T>> GetManyAsync<T>(string route = default, NameValueCollection parameters = default)
        => GetAsync<IList<T>>(route, parameters);

    public Task<IList<T>> GetManyAsync<T>(string route, string name, object? value)
        => GetAsync<IList<T>>(route, name, value);
}