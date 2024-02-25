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

    public static Dictionary<int, string> GetDictionary<T>(List<int> except = null)
        => Enum.GetNames(typeof(T)).ToDictionary(name => (int)Enum.Parse(typeof(T), name), name => name)
               .Where(w => !(except ?? new List<int>()).Contains(w.Key))
           .ToDictionary(k => k.Key, v => v.Value);

    public static IEnumerable<int> Invert<T>(this IEnumerable<int> selected, IEnumerable<int> except = null)
        => GetDictionary<T>().Select(s => s.Key)
                .Where(w => !(except ?? new List<int>()).Contains(w))
            .Except(selected.Select(s => s));

    public static string Join(this IEnumerable<object> vs, string separator = "") => string.Join(separator, vs.Select(s => s.ToString()));

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> other)
        where TKey : notnull
        where TValue : class
    {
        foreach(var values in other)
        {
            dict[values.Key] = values.Value;
        }
        return dict;
    }

    public static IEnumerable<int> RangeInclusive(int startNum, int endNum) => Enumerable.Range(startNum, endNum - startNum + 1);

    public static Dictionary<K, List<T>> Merge<K, T>(this Dictionary<K, List<T>> dict1, Dictionary<K, List<T>> dict2)
        where K : notnull
    => dict1.Concat(dict2)
        .GroupBy(kvp => kvp.Key)
        .ToDictionary(
            group => group.Key,
            group => group.SelectMany(kvp => kvp.Value).ToList());
}