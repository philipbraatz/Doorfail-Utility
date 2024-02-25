using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Doorfail.Utils.Extensions;

namespace Doorfail.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName<TEnum>(this TEnum enumValue)
            where TEnum : Enum
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .GetName();
        }

        public static string ToTitleCase<TEnum>(this TEnum str)
             where TEnum : Enum
            => str.ToString().ToTitleCase();

        public static string[] GetEnumKeys<TEnum>() => Enum.GetNames(typeof(TEnum));

        public static int[] GetEnumValues<TEnum>() => (int[])Enum.GetValues(typeof(TEnum));

        public static Dictionary<int, string> GetEnumCollection<TEnum>()
            => GetEnumValues<TEnum>().Zip(GetEnumKeys<TEnum>(), (x, y) =>
                new KeyValuePair<int, string>(x, y)).ToDictionary();
    }
}