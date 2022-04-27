using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Doorfail.Core.Utils.Extensions
{
    internal static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .GetName();
        }
        public static string[] GetEnumKeys<TEnum>() => Enum.GetNames(typeof(TEnum));
        public static int[] GetEnumValues<TEnum>() => (int[])Enum.GetValues(typeof(TEnum));
        public static Dictionary<int, string> GetEnumCollection<TEnum>()
            => GetEnumValues<TEnum>().Zip(GetEnumKeys<TEnum>(), (x, y) =>
                new KeyValuePair<int, string>(x, y)).ToDictionary();

    }
}
