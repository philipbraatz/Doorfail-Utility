using System.Collections.Specialized;
using RestSharp;

namespace Doorfail.Core.Client;

public partial class DoorClient
{
    public T Post<T>(string route = default, NameValueCollection parameters = default, object body = null)
    where T : class
    {
        var r = BuildRequest(route, Method.Post, parameters);
        if(body is not null)
        { r.AddJsonBody(body); }

        return HandleResponse(this.Execute<T>(r));
    }

    public async Task<T> PostAsync<T>(string route = default, NameValueCollection parameters = default, object body = null)
    {
        var r = BuildRequest(route, Method.Post, parameters);
        if(body is not null)
        { r.AddJsonBody(body); }

        return await HandleResponse(this.ExecuteAsync<T>(r));
    }

    public async Task<T> PostAsync<T>(string route, string name, object value, object body = null)
    {
        var r = BuildRequest(route, Method.Post, name, value);
        AddJsonBody(r, body);

        return await HandleResponse(this.ExecuteAsync<T>(r));
    }

    public async Task PostAsync(string route = default, NameValueCollection parameters = default, object body = null)
    {
        var r = BuildRequest(route, Method.Post, parameters);
        if(body is not null)
        { r.AddJsonBody(body); }

        await HandleResponse(this.ExecuteAsync(r));
    }

    public async Task PostAsync(string route, string name, object value, object body = null)
    {
        var r = BuildRequest(route, Method.Post, name, value);
        if(body is not null)
        { r.AddJsonBody(body); }

        await HandleResponse(this.ExecuteAsync(r));
    }

    public async Task<IEnumerable<T>> PostBatch<T>(string route, IEnumerable<object> body, int batchSize, NameValueCollection parameters = default)
    {
        var resultList = new List<T>();

        var stack = new Stack<object>(body);

        var exceptions = new List<Exception>();
        var failedRange = new List<Tuple<int, int>>();

        do
        {
            var batch = new List<object>();
            for(int i = 0; i < batchSize && stack.Count > 0; i++)
            { batch.Add(stack.Pop()); }

            try
            {
                var result = await PostManyAsync<T>(route, parameters, batch);
                resultList.AddRange(result);

                if(result.Count() < batch.Count())
                {
                    _logger.Warn($"{route} - failed to process {batch.Count() - result.Count()} entries in batch");
                }
            } catch(Exception e)
            {
                failedRange.Add(new Tuple<int, int>(Math.Max(0, body.Count() - stack.Count - batchSize), body.Count() - stack.Count));
                exceptions.Add(e);
            }
        } while(stack.Any());

        if(exceptions.Any())
        {
            _logger.Warn(new AggregateException($"Failed processing batch {string.Join(", ", failedRange.Select(s => s.Item1 + "-" + s.Item2))}", exceptions));
            throw new AggregateException(exceptions);
        }

        return resultList;
    }

    public async Task<IDictionary<K, V>> PostBatchDictonary<K, V>(string route, IDictionary<object, object> body, int batchSize, NameValueCollection parameters = default)
        where K : notnull
        where V : notnull
    {
        var resultList = new List<IDictionary<K, V>>();

        var stack = new Stack<KeyValuePair<object, object>>(body.ToArray());

        var exceptions = new List<Exception>();
        var failedRange = new List<Tuple<int, int>>();

        do
        {
            var batch = new List<object>();
            for(int i = 0; i < batchSize && stack.Count > 0; i++)
            { batch.Add(stack.Pop()); }

            try
            {
                var result = await PostDictionaryAsync<K, V>(route, parameters, batch);
                resultList.Add(result);

                if(result.Count() < batch.Count())
                {
                    _logger.Warn($"{route} - failed to process {batch.Count() - result.Count()} entries in batch");
                }
            } catch(Exception e)
            {
                failedRange.Add(new Tuple<int, int>(Math.Max(0, body.Count - stack.Count - batchSize), body.Count - stack.Count));
                exceptions.Add(e);
            }
        } while(stack.Any());

        if(exceptions.Any())
        {
            _logger.Warn(new AggregateException($"Failed processing batch {string.Join(", ", failedRange.Select(s => s.Item1 + "-" + s.Item2))}", exceptions));
            throw new AggregateException(exceptions);
        }

        IDictionary<K, V> results = resultList.SelectMany(s => s).ToDictionary(k => k.Key, v => v.Value);
        return results;
    }

    public async Task<IDictionary<K, V>> PostBatchDictonary<K, V>(string route, IEnumerable<object> batchBody, int batchSize, object body = null, NameValueCollection parameters = default)
        where K : notnull
        where V : notnull
    {
        IEnumerable<KeyValuePair<K, V>> resultList = new List<KeyValuePair<K, V>>();

        var stack = new Stack<object>(batchBody);

        var exceptions = new List<Exception>();
        var failedRange = new List<Tuple<int, int>>();

        do
        {
            var batch = new List<object>();
            for(int i = 0; i < batchSize && stack.Count > 0; i++)
            { batch.Add(stack.Pop()); }

            try
            {
                var result = await PostDictionaryAsync<K, V>(route, parameters, new object[] { batch, body });
                resultList = resultList.Concat(result);

                if(result.Count() < batch.Count())
                {
                    _logger.Warn($"{route} - failed to process {batch.Count() - result.Count()} entries in batch");
                }
            } catch(Exception e)
            {
                failedRange.Add(new Tuple<int, int>(Math.Max(0, batchBody.Count() - stack.Count - batchSize), batchBody.Count() - stack.Count));
                exceptions.Add(e);
            }
        } while(stack.Any());

        if(exceptions.Any())
        {
            _logger.Warn(new AggregateException($"Failed processing batch {string.Join(", ", failedRange.Select(s => s.Item1 + "-" + s.Item2))}", exceptions));
            throw new AggregateException(exceptions);
        }

        return resultList.ToDictionary(k => k.Key, v => v.Value);
    }

    public IDictionary<K, V> PostDictionary<K, V>(string route = default, NameValueCollection parameters = default, object body = null)
        where K : notnull
        where V : notnull
        => Post<IDictionary<K, V>>(route, parameters, body);

    public Task<IDictionary<K, V>> PostDictionaryAsync<K, V>(string route = default, NameValueCollection parameters = default, object body = null)
        where K : notnull
        where V : notnull
        => PostAsync<IDictionary<K, V>>(route, parameters, body);

    public Task<IDictionary<K, V>> PostDictionaryAsync<K, V>(string route, string name, object value, object body = null)
        where K : notnull
        where V : notnull
        => PostAsync<IDictionary<K, V>>(route, name, value, body);

    public Task<IList<T>> PostManyAsync<T>(string route = default, NameValueCollection parameters = default, object body = null)
        => PostAsync<IList<T>>(route, parameters, body);

    public Task<IList<T>> PostManyAsync<T>(string route, string name, object value, object body = null)
        => PostAsync<IList<T>>(route, name, value, body);
}