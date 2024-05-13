using Doorfail.Utils.Extensions;

namespace Doorfail.Utils.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
            => keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey, TValue)> keyValuePairs)
            => keyValuePairs.ToDictionary(kv => kv.Item1, kv => kv.Item2);

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> keyValuePairs)
            => keyValuePairs.ToDictionary(kv => kv.Item1, kv => kv.Item2);

        public static IDictionary<string, string> ToStringDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
            => keyValuePairs.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());

        public static IDictionary<string, string> ToStringDictionary<TValue>(this IEnumerable<KeyValuePair<string, TValue>> keyValuePairs)
            => keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

        public static IDictionary<string, string> ToStringDictionary<TKey, TValue>(this IDictionary<TKey, TValue> keyValuePairs)
            => keyValuePairs.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());

        public static IDictionary<string, string> ToStringDictionary<TValue>(this IDictionary<string, TValue> keyValuePairs)
            => keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

        public static Dictionary<int, string> GetDictionary<T>(List<int> except = null)
            => Enum.GetNames(typeof(T)).ToDictionary(name => (int)Enum.Parse(typeof(T), name), name => name)
                   .Where(w => !(except ?? new List<int>()).Contains(w.Key))
               .ToDictionary(k => k.Key, v => v.Value);

        public static IEnumerable<int> Invert<T>(this IEnumerable<int> selected, IEnumerable<int> except = null)
=> GetDictionary<T>().Select(s => s.Key)
        .Where(w => !(except ?? new List<int>()).Contains(w))
    .Except(selected.Select(s => s));

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other)
            where TKey : notnull
            where TValue : class
        {
            foreach(var values in other)
                dict[values.Key] = values.Value;

            return dict;
        }

        public static IDictionary<K, List<T>> Merge<K, T>(this IDictionary<K, List<T>> dict1, IDictionary<K, List<T>> dict2)
            where K : notnull
        => dict1.Concat(dict2)
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(kvp => kvp.Value).ToList());
    }
}