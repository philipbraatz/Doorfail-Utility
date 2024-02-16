using System.Globalization;

namespace Doorfail.Core.Utils.Extensions;

internal class CultureExtensions
{
    public static string ToTitleCase(this string str)
        => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
}
