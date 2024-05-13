using System.Reflection;
using Doorfail.Utils;

namespace Doorfail.Utils.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool HasProperty(this Type obj, string propertyName) => obj.GetProperty(propertyName) != null;

        public static object GetValue<TEntity>(this TEntity obj, string propertyName)
        {
            if(string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(propertyName);
            var propinf = typeof(TEntity).GetProperties().Where(c => c.Name == propertyName).FirstOrDefault();
            return propinf != null ? propinf.GetValue(obj) ?? default : throw new PropertyException(typeof(TEntity), propertyName);
        }

        public static void SetValue<TEntity>(this TEntity obj, string propertyName, object value)
        {
            var propinf = typeof(TEntity).GetProperties().Where(c => c.Name == propertyName).FirstOrDefault();
            if(propinf != null)
                propinf.SetValue(obj, value);
            else
                throw new PropertyException(typeof(TEntity), propertyName);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this PropertyInfo property, bool inherit)
            where TAttribute : class
        => property.GetCustomAttributes(inherit).FirstOrDefault(f => f is TAttribute) as TAttribute;

        public static void Populate<T>(this T obj, IDictionary<string, string> values, Func<string, Type, object> convertStringToType = null)
            where T : class
        {
            convertStringToType ??= Convert.ChangeType;
            var properties = typeof(T).GetProperties();

            foreach(var (key, value) in values)
            {
                var property = Array.Find(properties, p => p.Name == key);
                if(property is not null)
                {
                    try
                    {
                        property.SetValue(obj, convertStringToType(value, property.PropertyType) ?? value);
                    } catch(Exception ec)
                    {
                        Console.WriteLine($"[{key}]={value}: {ec.Message}");
                    }
                }
            }
        }
    }
}