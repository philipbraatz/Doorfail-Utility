using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.Interactivity.Enums;
using Doorfail.Core.Attributes;
using Doorfail.Utils;
using Doorfail.Utils.Extensions;

namespace Doorfail.Core.Components;

public class PagedModalBuilder<T>(BaseDiscordClient client, ResourceManager resource, CultureInfo culture) where T : class
{
    private readonly BaseDiscordClient client = client;
    private readonly ResourceManager resources = resource;
    private readonly CultureInfo culture = culture;

    private PositionalComponent GetPositionalComponentFromProperty(PropertyInfo p, T defaultValues = null, ulong? userId = 0)
    {
        var rawCustomId = p.GetCustomAttribute<CustomIdAttribute>().Id;

        var customId = resources.GetString(rawCustomId, CultureInfo.InvariantCulture);
        if(string.IsNullOrEmpty(customId))
            throw new MissingResourceException(rawCustomId, CultureInfo.InvariantCulture);

        var pagePart = customId.Split("_").Skip(1).First()
            .Replace("page","")
            .Replace("p","")
            .Replace("modal","")
            .Replace("m", "")
            .Replace("section","")
            .Replace("s","");
        var orderPart = customId.Split("_").Last()
            .Replace("order","")
            .Replace("o","");

        var page = int.Parse(pagePart);
        var order = int.Parse(orderPart);

        var displayNameAtt = p.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        var displayName = resources.GetString(p.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName, culture);
        if(string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(displayName))
            throw new MissingResourceException(displayNameAtt, culture);

        var placeholderAtt = p.GetCustomAttribute<PlaceholderAttribute>()?.PlaceHolder;
        var placeholder = placeholderAtt.StartsWith("@") ? placeholderAtt[1..] : resources.GetString(placeholderAtt, culture);
        if(string.IsNullOrEmpty(placeholder))
            throw new MissingResourceException(placeholderAtt, culture);

        var required = p.GetCustomAttribute<RequiredAttribute>() != null;
        var maxLength = p.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength;
        var minLength = p.GetCustomAttribute<StringLengthAttribute>()?.MinimumLength;
        var defaultValue = p.GetCustomAttribute<DefaultValueAttribute>()?.Value.ToString();

        if((customId?.Contains("_m") ?? false) ||
            (customId?.Contains("_s") ?? false))
        {
            customId += userId.ToString();

            //var selectT = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(f => f.GetCustomAttribute<CustomIdAttribute>()?.Id == rawCustomId);
            // Split the string by semicolon to get individual currency entries
            var selectOptionsProperty = typeof(T).GetProperties()
                .FirstOrDefault(l => l.GetCustomAttribute<CustomIdAttribute>()?.Id == rawCustomId &&
                    l.PropertyType.IsArray)
                ?? throw new InvalidOperationException($"{typeof(T)} requires an array property with a matching attribute of {rawCustomId}.");

            // Get static property value
            var selectOptions = selectOptionsProperty.GetValue(defaultValues) as string[];

            defaultValue = defaultValues?.GetValue(p.Name)?.ToString() ?? defaultValue;

            var selectOptionComponents = selectOptions.Select(c =>
            {
                var parts = c.Trim().Split('|');
                var emoiji = parts.Last();
                var label = string.Join(' ', parts.Take(parts.Length - 1));
                var value = parts.Length == 3 ? parts[1] : parts[0];
                if(string.IsNullOrEmpty(emoiji) || string.IsNullOrEmpty(label) || string.IsNullOrEmpty(value))
                    throw new ArgumentException($"Invalid select option: {c}");

                var selected = false;
                if(value == defaultValue)
                    selected = true;
                try
                {
                    return new DiscordStringSelectComponentOption(label, value, DiscordEmoji.FromName(client, emoiji,false), selected);
                }
                catch
                {
                    Console.WriteLine(c);

                    return new DiscordStringSelectComponentOption(label, value, isDefault: selected);
                }
            });

            return new(new DiscordStringSelectComponent(displayName, selectOptionComponents, customId: customId), page, order);
        } else if(defaultValue == "Datetime(now)")
        {
            defaultValue = DateTime.Now.ToString();
        }

        defaultValue = defaultValues?.GetValue(p.Name)?.ToString() ?? defaultValue;
        DiscordTextComponent component = new(TextComponentStyle.Small, customId, displayName, placeholder,minLength, maxLength, required, defaultValue);

        // If Description exists, use it as AdditionalProperties["Title"]. Return component
        var title = p.GetCustomAttribute<DescriptionAttribute>()?.Description;
        if(title is not null)
            component.AdditionalProperties["Title"] = title;

        return new(component, page, order);
    }

    public IEnumerable<PositionalComponent> AsPositional(IEnumerable<DiscordComponent> components) => components.Select(AsPositional);
    public PositionalComponent AsPositional(DiscordComponent component)
    {
        var pagePart = component.CustomId.Split("_").Skip(1).First()
            .Replace("page","")
            .Replace("p","")
            .Replace("modal","")
            .Replace("m", "")
            .Replace("section","")
            .Replace("s","");
        var orderPart = component.CustomId.Split("_").Last()
            .Replace("order","")
            .Replace("o","");

        var page = int.Parse(pagePart);
        var order = int.Parse(orderPart);

        return new()
        {
            Component = component,
            Order = order,
            Page = page
        };
    }

    public IEnumerable<PositionalComponent> CreatePositionalComponents(T defaultValues = null, ulong? userId = 0)
        => typeof(T).GetProperties()
            .Where(w => w.GetCustomAttribute<CustomIdAttribute>() is not null
                && (w.PropertyType.IsAssignableFrom(typeof(string)) || !(w.PropertyType.IsArray || typeof(IEnumerable).IsAssignableFrom(w.PropertyType))))
            .Select(s => GetPositionalComponentFromProperty(s, defaultValues, userId));

    public List<ModalPage> BuildModalPages(string pageCustomId, IEnumerable<PositionalComponent> positionalComponents, int pageNum = 0)
    {
        return PositionalComponent.OrderComponents(positionalComponents).Select(s =>
        {
            var pageTitle = resource.GetString($"page{pageNum}_title", culture);
            if(string.IsNullOrEmpty(pageTitle))
                throw new MissingResourceException($"page{pageNum}_title", culture);


            var builder = new DiscordInteractionModalBuilder()
            .WithCustomId($"{pageCustomId}_p{pageNum++}")
            .WithTitle(pageTitle);

            foreach(var component in s)
                if(component is DiscordTextComponent text) // Only TextComponents supported currently
                    builder.AddTextComponent(text);

            return new ModalPage(builder);
        }).ToList();
    }
}