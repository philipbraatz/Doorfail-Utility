/// Know issues:
/// Multiple classes doing the same thing, should be refactored to a single class. 
/// This is because DisCatSharp Interaction and Message classes are not the same, and the methods are not the same.
using System.Globalization;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.Exceptions;
using Doorfail.Utils;
using NLog;
using LogLevel = NLog.LogLevel;

namespace Doorfail.Core;

public enum MessageStatus
{
    Waiting,
    Thinking,
    Response,
    Edit,
}

public interface IMessageFeed
{
    Task SetMessageStatus(MessageStatus status);

    Task EditFollowUp(ulong followupId, string message, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null);

    Task Edit(string message, IEnumerable<DiscordComponent> components = null, DiscordEmbed embed = null);

    Task SendFollowUp(string message, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null);

    Task Send(string message = null, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null);
}

public interface IUserMessageFeed
{

    Task EditFollowUp(ulong followupId, DiscordUser user, string resourceMessage, LogLevel logLevel, params string[] args);
    Task EditFollowUp(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage);

    Task Edit(DiscordUser user, string resourceMessage, LogLevel logLevel, params string[] args);
    Task Edit(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage);

    Task SendFollowUp(DiscordUser user, string resourceMessage, LogLevel logLevel, params string[] args);
    Task SendFollowUp(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage);

    Task Send(DiscordUser user, string resourceMessage, LogLevel logLevel, params string[] args);

    Task Send(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage);

}

public class BaseMessageFeed(BaseContext context) :IMessageFeed
{
    private readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly BaseContext context = context;

    private DiscordInteractionResponseBuilder BuildR(string message = null, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
    {
        components ??= new List<DiscordComponent>();

        var builder = new DiscordInteractionResponseBuilder();
        if(!string.IsNullOrEmpty(message))
            builder.WithContent(message);
        if(embed is not null)
            builder.AddEmbed(embed);

        if(components.Any())
        {
            foreach(var c in components)
            {
                if(c is DiscordActionRowComponent rowC)
                    builder.AddComponents((IEnumerable<DiscordActionRowComponent>)[rowC]);
                else
                    builder.AddComponents(c);
            }
        } else if(toUserOnly)
            builder.IsEphemeral = true;

        return builder;
    }

    private DiscordFollowupMessageBuilder BuildM(string message = null, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
    {
        components ??= new List<DiscordComponent>();

        var builder = new DiscordFollowupMessageBuilder();
        if(!string.IsNullOrEmpty(message))
            builder.WithContent(message);
        if(embed is not null)
            builder.AddEmbed(embed);

        if(components.Any())
        {
            foreach(var c in components)
            {
                if(c is DiscordActionRowComponent rowC)
                    builder.AddComponents((IEnumerable<DiscordActionRowComponent>)[rowC]);
                else
                    builder.AddComponents(c);
            }
        } else if(toUserOnly)
            builder.IsEphemeral = true;

        return builder;
    }

    private DiscordWebhookBuilder BuildW(string message = null, IEnumerable<DiscordComponent> components = null, DiscordEmbed? embed = null)
    {
        components ??= new List<DiscordComponent>();

        var builder = new DiscordWebhookBuilder();
        if(!string.IsNullOrEmpty(message))
            builder.WithContent(message);
        {
            if(embed is not null)
                builder.AddEmbed(embed);
            if(components.Any())
            {
                foreach(var c in components)
                    if(c is DiscordActionRowComponent rowC)
                        builder.AddComponents((IEnumerable<DiscordActionRowComponent>)[rowC]);
                    else
                        builder.AddComponents(c);
            }
        }

        return builder;
    }

    public async Task Send(string message = null, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
        => await LogBadRequests(() => context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, BuildR(message, components, toUserOnly, embed)));

    public async Task SendFollowUp(string message, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
                => await LogBadRequests(() => context.FollowUpAsync(BuildM(message, components, toUserOnly, embed)));
    public async Task SendFollowUp(string message, string fileName, FileStream file, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
        => await LogBadRequests(() => context.FollowUpAsync(BuildM(message, components, toUserOnly, embed).AddFile(fileName, file)));

    public async Task Edit(string message, IEnumerable<DiscordComponent> components = null, DiscordEmbed? embed = null)
       => await LogBadRequests(() => context.EditResponseAsync(BuildW(message, components, embed)));
    public async Task EditFollowUp(ulong followupId, string message, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed embed = null)
       => await LogBadRequests(() => context.EditFollowupAsync(followupId, BuildW(message, components, embed)));
    public Task SetMessageStatus(MessageStatus status) => status switch
    {
        MessageStatus.Thinking => context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource),
        MessageStatus.Waiting => context.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate),
        _ => throw new NotImplementedException()
    };

    private async Task LogBadRequests(Func<Task> messageAction)
    {
        try
        {
            await messageAction();
        } catch(BadRequestException re)
        {
            logger.Error(re, $"Failed to send message: {re.Errors}");
        }
    }
}

public class InteractionMessageFeed(DiscordInteraction interaction) :IMessageFeed
{
    private readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly DiscordInteraction interaction = interaction;

    private DiscordInteractionResponseBuilder BuildI(string message = null, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
    {
        components ??= new List<DiscordComponent>();

        var builder = new DiscordInteractionResponseBuilder();
        if(!string.IsNullOrEmpty(message))
            builder.WithContent(message);
        if(embed is not null)
            builder.AddEmbed(embed);

        if(components.Any())
        {
            foreach(var c in components)
            {
                if(c is DiscordActionRowComponent rowC)
                    builder.AddComponents((IEnumerable<DiscordActionRowComponent>)[rowC]);
                else
                    builder.AddComponents(c);
            }
        } else if(toUserOnly)
            builder.IsEphemeral = true;

        return builder;
    }

    private DiscordFollowupMessageBuilder BuildF(string message = null, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
    {
        components ??= new List<DiscordComponent>();

        var builder = new DiscordFollowupMessageBuilder();
        if(!string.IsNullOrEmpty(message))
            builder.WithContent(message);
        if(embed is not null)
            builder.AddEmbed(embed);
        if(components.Any())
        {
            foreach(var c in components)
            {
                if(c is DiscordActionRowComponent rowC)
                    builder.AddComponents(rowC);
                else
                    builder.AddComponents(c);
            }
        }
        if(toUserOnly)
            builder.IsEphemeral = true;

        return builder;
    }

    private DiscordWebhookBuilder BuildW(string message = null, IEnumerable<DiscordComponent> components = null, DiscordEmbed? embed = null)
    {
        components ??= new List<DiscordComponent>();

        var builder = new DiscordWebhookBuilder();
        if(!string.IsNullOrEmpty(message))
            builder.WithContent(message);
        if(embed is not null)
            builder.AddEmbed(embed);
        if(components.Any())
        {
            foreach(var c in components)
            {
                if(c is DiscordActionRowComponent rowC)
                    builder.AddComponents((IEnumerable<DiscordActionRowComponent>)[rowC]);
                else
                    builder.AddComponents(c);
            }
        }

        return builder;
    }

    public async Task Send(string message = null, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
       => await LogBadRequests(() => interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, BuildI(message, components, toUserOnly, embed)));

    public async Task SendFollowUp(string message, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
       => await LogBadRequests(() => interaction.CreateFollowupMessageAsync(BuildF(message, components, toUserOnly, embed)));

    public async Task SendFollowUp(string message, string fileName, FileStream file, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed? embed = null)
   => await LogBadRequests(() => interaction.CreateFollowupMessageAsync(BuildF(message, components, toUserOnly, embed).AddFile(fileName, file)));


    public async Task Edit(string message, IEnumerable<DiscordComponent> components = null, DiscordEmbed? embed = null)
       => await LogBadRequests(() => interaction.EditOriginalResponseAsync(BuildW(message, components, embed)));

    public async Task Edit(string message, string fileName, FileStream file, IEnumerable<DiscordComponent> components = null, DiscordEmbed? embed = null)
       => await LogBadRequests(() => interaction.EditOriginalResponseAsync(BuildW(message, components, embed).AddFile(fileName, file)));

    public async Task EditFollowUp(ulong followupId, string message, IEnumerable<DiscordComponent> components = null, bool toUserOnly = true, DiscordEmbed embed = null)
    => await LogBadRequests(() => interaction.EditFollowupMessageAsync(followupId, BuildW(message, components, embed)));


    public Task SetMessageStatus(MessageStatus status) => status switch
    {
        MessageStatus.Thinking => interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource),
        MessageStatus.Waiting => interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate),
        MessageStatus.Edit => interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage),
        _ => throw new NotImplementedException()
    };

    public Task Edit(AggregateException ea, string title = null, string message = null, string label = null, string customId = null, DiscordComponentEmoji emoji = null)
        => Edit(message, components:
            [
                new DiscordButtonComponent(ButtonStyle.Danger, customId: customId, label, emoji: emoji)
            ], embed: new DiscordEmbedBuilder()
            {
                Title = title,
                Description = string.Join("\n", ea.InnerExceptions.Select(s => s.Message)),
                Color = DiscordColor.Red
            });

    private async Task LogBadRequests(Func<Task> messageAction)
    {
        try
        {
            await messageAction();
        } catch(BadRequestException re)
        {
            logger.Error(re, $"Failed to send message: {re.Errors}");
        }
    }
}

public class CulturedMessageFeed(ResourceManager resource, BaseContext context) :IUserMessageFeed
{
    private readonly ResourceManager resource = resource;
    private readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly BaseMessageFeed context = new(context);

    private async Task Send(DiscordUser user, string resourceMessage, LogLevel? logLevel, Func<string, Task> send, params string[] args)
    {
        var logMessage = resource.GetString(resourceMessage+"-log", new(user.Locale));
        if(string.IsNullOrEmpty(logMessage))
            logMessage = resourceMessage+"-log";

        var message = resource.GetString(resourceMessage, new(user.Locale));
        if(string.IsNullOrEmpty(message))
            message = resourceMessage;

        if(logLevel is not null)
            logger.Log(logLevel, logMessage, args);

        await send(string.Format(message, args));
        if(message == resourceMessage)
            throw new MissingResourceException(resourceMessage, new CultureInfo(user.Locale));
    }

    public Task SetMessageStatus(MessageStatus status) => context.SetMessageStatus(status);

    public Task Send(DiscordUser user, string resourceMessage, LogLevel? logLevel, params string[] args) => Send(user, resourceMessage, logLevel, message => context.Send(message), args);
    public Task Send(DiscordUser user, string resourceMessage, DiscordEmbed embed, LogLevel? logLevel, params string[] args) => Send(user, resourceMessage, logLevel, message => context.Send(message, embed: embed), args);
    public Task Send(DiscordUser user, string resourceMessage, IEnumerable<DiscordComponent> components, LogLevel? logLevel, params string[] args) => Send(user, resourceMessage, logLevel, message => context.Send(message, components), args);
    public Task Send(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage)
        => Send(user, resourceMessage, LogLevel.Error, guildName, interactionId.ToString(), exception.Message, exception.StackTrace, exception.InnerException?.Message);

    public async Task SendFollowUp(DiscordUser user, string resourceMessage, LogLevel logLevel, params string[] args) => await Send(user, resourceMessage, logLevel, message => context.SendFollowUp(message), args);
    public async Task SendFollowUp(DiscordUser user, string resourceMessage, string filename, FileStream file, LogLevel logLevel, params string[] args)
        => await Send(user, resourceMessage, logLevel, message => context.SendFollowUp(message, filename, file), args);
    public Task SendFollowUp(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage)
        => Send(user, resourceMessage, LogLevel.Error, guildName, interactionId.ToString(), exception.Message, exception.StackTrace, exception.InnerException?.Message);

    public async Task Edit(DiscordUser user, string resourceMessage, LogLevel logLevel, params string[] args) => await Send(user, resourceMessage, logLevel, message => context.Edit(message), args);
    public Task Edit(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage)
        => Send(user, resourceMessage, LogLevel.Error, guildName, interactionId.ToString(), exception.Message, exception.StackTrace, exception.InnerException?.Message);
    public async Task Edit(DiscordUser user, string resourceMessage, IEnumerable<DiscordComponent> components, DiscordEmbed? embed, LogLevel logLevel, params string[] args)
        => await Send(user, resourceMessage, logLevel, message => context.Edit(message, components, embed), args);

    public async Task EditFollowUp(ulong followupId, DiscordUser user, string resourceMessage, LogLevel logLevel, params string[] args) => await Send(user, resourceMessage, logLevel, message => context.EditFollowUp(followupId, message), args);
    public Task EditFollowUp(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage)
        => Send(user, resourceMessage, LogLevel.Error, guildName, interactionId.ToString(), exception.Message, exception.StackTrace, exception.InnerException?.Message);

    public Task Edit(DiscordUser user, AggregateException ea, LogLevel logLevel, string resourceTitle, string resourceMessage, string label, string customId, DiscordComponentEmoji? emoji, params string[] args)
        => Edit(user, resourceMessage, components:
            [
                new DiscordButtonComponent(ButtonStyle.Danger, customId: customId, label, emoji: emoji)
            ], embed: new DiscordEmbedBuilder()
            {
                Title = resourceTitle,
                Description = string.Join("\n", ea.InnerExceptions.Select(s => s.Message)),
                Color = DiscordColor.Red
            }, logLevel, args);
}

public class CulturedInteractionMessageFeed(ResourceManager resource, DiscordInteraction interaction) :IUserMessageFeed
{
    private readonly ResourceManager resource;
    private readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly InteractionMessageFeed interaction = new(interaction);

    public Task SetMessageStatus(MessageStatus status) => interaction.SetMessageStatus(status);

    private async Task Send(DiscordUser user, string resourceMessage, LogLevel? logLevel, Func<string, Task> send, params string[] args)
    {
        var logMessage = resource.GetString(resourceMessage+"-log", new(user.Locale));
        if(string.IsNullOrEmpty(logMessage))
            logMessage = resourceMessage+"-log";

        var message = resource.GetString(resourceMessage, new(user.Locale));
        if(string.IsNullOrEmpty(message))
            message = resourceMessage;

        if(logLevel is not null)
            logger.Log(logLevel, logMessage, args);

        await send(string.Format(message, args));
        if(message == resourceMessage)
            throw new MissingResourceException(resourceMessage, new CultureInfo(user.Locale));
    }

    public Task Send(DiscordUser user, string resourceMessage, LogLevel? logLevel, params string[] args) => Send(user, resourceMessage, logLevel, message => interaction.Send(message), args);
    public Task Send(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage)
        => Send(user, resourceMessage, LogLevel.Error, guildName, interactionId.ToString(), exception.Message, exception.StackTrace, exception.InnerException?.Message);

    public async Task SendFollowUp(DiscordUser user, string resourceMessage, LogLevel? logLevel, params string[] args) => await Send(user, resourceMessage, logLevel, message => interaction.SendFollowUp(message), args);
    public async Task SendFollowUp(DiscordUser user, string resourceMessage, DiscordEmbed embed, LogLevel? logLevel, params string[] args) => await Send(user, resourceMessage, logLevel, message
        => interaction.SendFollowUp(message, embed: embed), args);
    public async Task SendFollowUp(DiscordUser user, string resourceMessage, string filename, FileStream file, LogLevel? logLevel, params string[] args)
        => await Send(user, resourceMessage, logLevel, message => interaction.SendFollowUp(message, filename, file), args);
    public Task SendFollowUp(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage)
        => Send(user, resourceMessage, LogLevel.Error, guildName, interactionId.ToString(), exception.Message, exception.StackTrace, exception.InnerException?.Message);

    public async Task Edit(DiscordUser user, string resourceMessage, LogLevel logLevel, params string[] args) => await Send(user, resourceMessage, logLevel, message => interaction.Edit(message), args);
    public async Task Edit(DiscordUser user, string resourceMessage, string fileName, FileStream file, LogLevel logLevel, params string[] args) => await Send(user, resourceMessage, logLevel, message
            => interaction.Edit(message, fileName, file), args);
    public async Task Edit(DiscordUser user, string resourceMessage, DiscordEmbed embed, LogLevel logLevel, params string[] args) => await Send(user, resourceMessage, logLevel, message => interaction.Edit(message, embed: embed), args);
    public Task Edit(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage)
        => Send(user, resourceMessage, LogLevel.Error, guildName, interactionId.ToString(), exception.Message, exception.StackTrace, exception.InnerException?.Message);
    public async Task Edit(DiscordUser user, string resourceMessage, IEnumerable<DiscordComponent> components, DiscordEmbed? embed, LogLevel logLevel, params string[] args)
    => await Send(user, resourceMessage, logLevel, message => interaction.Edit(message, components, embed), args);
    public Task Edit(DiscordUser user, AggregateException ea, LogLevel logLevel, string resourceTitle, string resourceMessage, string label, string customId, DiscordComponentEmoji? emoji, params string[] args)
    {
        ea.InnerExceptions.ToList().ForEach(s => logger.Warn(s.Message));

        return Edit(user, resourceMessage, components:
            [
                new DiscordButtonComponent(ButtonStyle.Danger, customId: customId, label, emoji: emoji)
            ], embed: new DiscordEmbedBuilder()
            {
                Title = resourceTitle,
                Description = string.Join("\n", ea.InnerExceptions.Select(s => s.Message)),
                Color = DiscordColor.Red
            }, logLevel, args);
    }

    public async Task EditFollowUp(ulong followupId, DiscordUser user, string resourceMessage, LogLevel logLevel, params string[] args) => await Send(user, resourceMessage, logLevel, message => interaction.EditFollowUp(followupId, message), args);
    public Task EditFollowUp(Exception exception, DiscordUser user, ulong interactionId, string guildName, string resourceMessage)
        => Send(user, resourceMessage, LogLevel.Error, guildName, interactionId.ToString(), exception.Message, exception.StackTrace, exception.InnerException?.Message);
}