using System.Collections.Generic;
using System.Linq;

namespace Doorfail.Core.Utils.Extensions
{
    public static class DoorfailExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
            => keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);

        public static string Join(this IEnumerable<object> vs, string separator = "") => string.Join(separator, vs.Select(s => s.ToString()));
    }
}