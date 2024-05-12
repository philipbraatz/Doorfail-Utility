using System.Globalization;

namespace Doorfail.Utils.Extensions;

internal static class CultureExtensions
{
    public static string ToTitleCase(this string str)
        => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
}
