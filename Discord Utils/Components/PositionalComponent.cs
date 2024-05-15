using DisCatSharp.Entities;

namespace Doorfail.Core.Components;

public struct PositionalComponent(DiscordComponent component, int page, int order)
{
    public DiscordComponent Component { get; set; } = component;
    public int Page { get; set; } = page;
    public int Order { get; set; } = order;

    public static IEnumerable<DiscordComponent[]> OrderComponents(IEnumerable<PositionalComponent> components)
        => components.GroupBy(c => c.Page)
            .OrderBy(c => c.Key)
            .Select(c =>
                c.OrderBy(c => c.Order)
                .Select(c => c.Component).ToArray());
}