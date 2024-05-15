namespace Doorfail.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CustomIdAttribute(string id) :Attribute
{
    public string Id { get; } = id;
}

[AttributeUsage(AttributeTargets.Property)]
public class PlaceholderAttribute(string id) :Attribute
{
    public string PlaceHolder { get; } = id;
}