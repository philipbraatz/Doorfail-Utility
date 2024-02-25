using System.Reflection;

namespace Doorfail.Utils.Extensions;

public static class ReflectionExtensions
{
    public static bool HasProperty(this Type obj, string propertyName) => obj.GetProperty(propertyName) != null;

    public static object GetValue<TEntity>(this TEntity obj, string propertyName)
    {
        var propinf = typeof(TEntity).GetProperties().FirstOrDefault(c => c.Name == propertyName);
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
}