using Doorfail.Utils.Extensions;

namespace Doorfail.Utils.Extensions;

public static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        => keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);

    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey, TValue)> keyValuePairs)
        => keyValuePairs.ToDictionary(kv => kv.Item1, kv => kv.Item2);

    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> keyValuePairs)
        => keyValuePairs.ToDictionary(kv => kv.Item1, kv => kv.Item2);

    public static Dictionary<string, string> ToStringDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        => keyValuePairs.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());

    public static Dictionary<string, string> ToStringDictionary<TValue>(this IEnumerable<KeyValuePair<string, TValue>> keyValuePairs)
        => keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

    public static Dictionary<string, string> ToStringDictionary<TKey, TValue>(this Dictionary<TKey, TValue> keyValuePairs)
        => keyValuePairs.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());

    public static Dictionary<string, string> ToStringDictionary<TValue>(this Dictionary<string, TValue> keyValuePairs)
        => keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

    public static string Join(this IEnumerable<object> vs, string separator = "") => string.Join(separator, vs.Select(s => s.ToString()));
}