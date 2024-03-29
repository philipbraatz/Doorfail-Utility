namespace Doorfail.Core.Util.Extensions;

public static class ListExtensions
{
    public static string Join(this IEnumerable<object> vs, string separator = "") => string.Join(separator, vs.Select(s => s.ToString()));

    public static IEnumerable<int> RangeInclusive(int startNum, int endNum) => Enumerable.Range(startNum, endNum - startNum + 1);

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> list) => list.SelectMany(s => s);
}