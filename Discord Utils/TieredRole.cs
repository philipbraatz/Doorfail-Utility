using DisCatSharp.Entities;

namespace Doorfail.Core;

public class TieredRole(List<DiscordRole> OrderedRoles)
{
    public readonly List<DiscordRole> Roles = OrderedRoles;

    // Move up in the role hierarchy.
    public async Task UpgradeRole(DiscordMember member, string reason)
    {
        var newRole = GetNextRole(member, false)?? throw new Exception($"Cannot upgrade role. Already at '{GetCurrentRole(member)!.Name}', the highest role.");
        if(newRole is null)
            return;
        var currentRole = GetCurrentRole(member);
        if(currentRole is not null)
            await member.RevokeRoleAsync(GetCurrentRole(member)!, reason);

        await member.GrantRoleAsync(newRole, reason);
    }

    // Move down in the role hierarchy.
    public async Task DowngradeRole(DiscordMember member, string reason)
    {
        var newRole = GetNextRole(member, true);
        if(newRole is null)
        {
            var currentRole = GetCurrentRole(member);
            if(currentRole is not null)
                await member.RevokeRoleAsync(GetCurrentRole(member)!, reason);

            return;
        }

        await member.GrantRoleAsync(newRole, reason);
    }

    // Get Current role in the hierarchy.
    public DiscordRole? GetCurrentRole(DiscordMember member) => member.Roles.FirstOrDefault(Roles.Contains);

    // Get the next role in the hierarchy.
    public DiscordRole? GetNextRole(DiscordMember member, bool previous = false)
    {
        var currentRole = GetCurrentRole(member);
        var currentIndex = Roles.IndexOf(currentRole);
        var nextIndex = currentIndex + (previous ? -1 : 1);

        return nextIndex >= 0 && nextIndex < Roles.Count ? Roles[nextIndex] : null;
    }

    public async Task SetRole(DiscordMember member, DiscordRole role, string reason)
    {
        var currentRole = GetCurrentRole(member);
        if(currentRole is not null)
            await member.RevokeRoleAsync(currentRole, reason);

        await member.GrantRoleAsync(role, reason);
    }
}