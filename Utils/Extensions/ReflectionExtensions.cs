using System;
using System.Linq;
using System.Reflection;

namespace Doorfail.Core.Utils.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }

        public static object GetValue<TEntity>(this TEntity obj, string propertyName)
        {
            PropertyInfo propinf = typeof(TEntity).GetProperties().Where(c => c.Name == propertyName).FirstOrDefault();
            if (propinf != null)
                return propinf.GetValue(obj) ?? default;
            else
                throw new PropertyException(typeof(TEntity), propertyName);
        }
        public static void SetValue<TEntity>(this TEntity obj, string propertyName, object value)
        {
            PropertyInfo propinf = typeof(TEntity).GetProperties().Where(c => c.Name == propertyName).FirstOrDefault();
            if (propinf != null)
                propinf.SetValue(obj, value);
            else
                throw new PropertyException(typeof(TEntity), propertyName);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this PropertyInfo property, bool inherit)
            where TAttribute : class
        => property.GetCustomAttributes(inherit).FirstOrDefault(f => f as TAttribute != null) as TAttribute;
    }
}