namespace Doorfail.Database.Internal;

[AttributeUsage(AttributeTargets.Property)]
public class ForeignLinkAttribute(string table) : Attribute
{
    public string Table = table;
}