using System.Text.RegularExpressions;

namespace Doorfail.Utils.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] EndOfLineChars = ['\r', '\n'];

        public static string Repeat(this char s, int times) => new(s, times);

        public static string Repeat(this string s, int times)
            => Enumerable.Range(1, times).Aggregate("", (a, i) => a+s);

        private static string ReturnIfEmpty(this string s, Func<string, string> action) => string.IsNullOrEmpty(s) ? string.Empty : action(s);

        public static IEnumerable<string> ToLines(this string s, StringSplitOptions options = StringSplitOptions.None) => s.Split(EndOfLineChars, options);

        public static string ToCamelCase(this string s) => s.ReturnIfEmpty(a => char.ToLower(a[0]) + a.Substring(1));

        public static string ToPascalCase(this string s) => s.ReturnIfEmpty(a => char.ToUpper(a[0]) + a.Substring(1));

        public static string ToSnakeCase(this string s) => s.ReturnIfEmpty(a => string.Concat(a.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower());
        
        public static Stream ToStream(this string s)
        {
            var stream = new MemoryStream();
            using(var writer = new StreamWriter(stream))
            {
                writer.Write(s);
                writer.Flush();
                stream.Position = 0;
            }
            return stream;
        }

        public static int? ToInt(this string text)
        {
            var isValid = int.TryParse(text, out var temp);
            return isValid ? temp : null;
        }

        public static string Join(this IEnumerable<string> list, string separator = ", ") => string.Join(separator, list);

        public static string Join(this IEnumerable<string> list, char separator = ',') => string.Join(separator.ToString(), list);

        public static string RemoveNonAlpha(this string input) => Regex.Replace(input, "[^a-zA-Z]", "");
    }
}