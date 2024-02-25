using System.Runtime.Serialization;
using Doorfail.Utils.Extensions;

namespace Doorfail.Core.Data;

internal enum EntityCode
{
    NOT_UNIQUE = -14,
    NOT_DELETED = -13,
    NOT_UPDATED = -12,
    NOT_INSERTED = -11,
    ID_NOT_FOUND = -10,
    DISABLED = -4,
    DELETED = -3,
    UPDATED = -2,
    INSERTED = -1,
}

internal class EntityException :Exception
{
    private EntityCode Code { get; set; }
    private string ClassName { get; set; }

    public EntityException(string className, EntityCode code)
    {
        Code = code;
    }

    public EntityException(string className, EntityCode code, string message) : base(message)
    {
        Code = code;
    }

    public EntityException(string className, EntityCode code, string message, Exception innerException) : base(message, innerException)
    {
        Code = code;
    }

    public override string Message
    {
        get
        {
            switch(Code)
            {
                case EntityCode.INSERTED:
                case EntityCode.UPDATED:
                case EntityCode.DELETED:
                    return $"Cannot {EnumExtensions.ToTitleCase(Code)} {ClassName}";

                case EntityCode.DISABLED:
                    return $"You do not have permission to modify a {Code.ToTitleCase()} {ClassName}";

                case EntityCode.ID_NOT_FOUND:
                    return $"{ClassName} does not have an entity with that Id";

                case EntityCode.NOT_INSERTED:
                    return $"Failed to Insert entity";

                case EntityCode.NOT_UPDATED:
                    return $"Failed to Update entity";

                case EntityCode.NOT_DELETED:
                    return $"Failed to Delete entity";

                case EntityCode.NOT_UNIQUE:
                    return $"{ClassName} already exists within the database";
            };
        }
    }

    public override string ToString()
    {
        return base.ToString();
    }
}