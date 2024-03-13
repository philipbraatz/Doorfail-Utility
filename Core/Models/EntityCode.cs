using Doorfail.Utils.Extensions;

namespace Doorfail.Core.Models;

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

    public override string Message => Code switch
    {
        EntityCode.INSERTED => $"Cannot {Code.ToTitleCase()} {ClassName}",
        EntityCode.UPDATED => $"Cannot {Code.ToTitleCase()} {ClassName}",
        EntityCode.DELETED => $"Cannot {Code.ToTitleCase()} {ClassName}",

        EntityCode.DISABLED => $"You do not have permission to modify a {Code.ToTitleCase()} {ClassName}",

        EntityCode.ID_NOT_FOUND => $"{ClassName} does not have an entity with that Id",

        EntityCode.NOT_INSERTED => $"Failed to Insert entity",

        EntityCode.NOT_UPDATED => $"Failed to Update entity",

        EntityCode.NOT_DELETED => $"Failed to Delete entity",

        EntityCode.NOT_UNIQUE => $"{ClassName} already exists within the database",
        _ => throw new NotImplementedException()
    };

    public override string ToString()
    {
        return base.ToString();
    }
}