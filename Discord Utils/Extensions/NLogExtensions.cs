using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using Doorfail.Utils.Extensions;
using NLog;

namespace Doorfail.Core.Extensions;

public static class NLogExtensions
{
    public static void SetGuildProperties(this Logger logger, DiscordGuild guild)
    {
        logger.SetProperty("GuildId", guild.Id.ToString());
        logger.SetProperty("GuildName", guild.Name);
    }
    public static void SetInteractionProperties(this Logger logger, InteractionContext ctx)
    {
        logger.SetProperty("GuildId", ctx.Guild.Id.ToString());
        logger.SetProperty("GuildName", ctx.Guild.Name);
        logger.SetProperty("ChannelId", ctx.Channel.Id.ToString());
        logger.SetProperty("ChannelName", ctx.Channel.Name);
        logger.SetValue("UserId", ctx.User.Id);
        logger.SetValue("Username", ctx.User.Username);
        logger.SetValue("SlashCommandId", ctx.CommandId);
        logger.SetValue("SlashCommand", ctx.CommandName);
        logger.SetValue("InteractionId", ctx.Interaction.Id);
    }

    public static void SetInteractionProperties(this Logger logger, ComponentInteractionCreateEventArgs v)
    {
        logger.SetProperty("GuildId", v.Guild.Id.ToString());
        logger.SetProperty("GuildName", v.Guild.Name);
        logger.SetProperty("ChannelId", v.Channel.Id.ToString());
        logger.SetProperty("ChannelName", v.Channel.Name);
        logger.SetValue("UserId", v.User.Id);
        logger.SetValue("Username", v.User.Username);
        logger.SetValue("ComponentId", v.Id);
    }
}
