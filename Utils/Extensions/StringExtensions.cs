using System.Globalization;
using System.Text.RegularExpressions;

namespace Doorfail.Core.Util.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] EndOfLineChars = { '\r', '\n' };

        public static string Repeat(this char s, int times)
            => new string(s, times);

        public static string Repeat(this string s, int times)
            => Enumerable.Range(1, times).Aggregate("", (a, i) => a+s);

        public static IEnumerable<string> ToLines(this string s, StringSplitOptions options = StringSplitOptions.None)
        {
            return s.Split(EndOfLineChars, options);
        }

        public static string ToCamelCase(this string s)
        {
            if(string.IsNullOrEmpty(s))
                return s;
            return char.ToLower(s[0]) + s.Substring(1);
        }

        public static string ToPascalCase(this string s)
        {
            if(string.IsNullOrEmpty(s))
                return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string ToTitleCase(this string str)
            => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());

        public static string ToSnakeCase(this string s)
        {
            if(string.IsNullOrEmpty(s))
                return s;
            return string.Concat(s.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

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

        public static int ToInt(this string text)
        {
            int temp;
            int.TryParse(text, out temp);
            return temp;
        }

        public static string Join(this IEnumerable<string> list, string separator = ", ")
        {
            return string.Join(separator, list);
        }

        public static string Join(this IEnumerable<string> list, char separator = ',')
        {
            return string.Join(separator.ToString(), list);
        }

        public static string RemoveNonAlpha(this string input) => Regex.Replace(input, "[^a-zA-Z]", "");
    }
}