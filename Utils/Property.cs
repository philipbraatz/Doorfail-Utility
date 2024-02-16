using System.Reflection;
using System.Runtime.Serialization;

namespace Doorfail.Utils;

//PropertyInfo with custom info
//Max
public class PropertyDB_Info<TEntity>
{
    public int Max { get; private set; }
    public PropertyInfo Info;

    public PropertyDB_Info(PropertyInfo info, TEntity entity)
    {
        Info = info;
        loadPropertyMax(entity);
    }

    private void loadPropertyMax(TEntity entity)
    {
        if(Info.Name != "String")
            Max = -1;
        else
        {
            //int? nmax = context.GetMaxLength<TEntity>((x) => (string)Info.GetValue(entity));
            //Max = nmax != null ? (int)nmax : -1;
        }
    }
}

public class PropertyException :Exception
{
    public PropertyException() : base()
    {
    }

    public PropertyException(Type tEntity, string propertyName) : base(tEntity + " does not have the property " + propertyName)
    {
    }

    public PropertyException(Type tEntity, string propertyName, Exception innerException) : base(tEntity + " does not have the property " + propertyName, innerException)
    {
    }

    protected PropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}