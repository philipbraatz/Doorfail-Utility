using System.Collections.Concurrent;
using DisCatSharp.Entities;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Targets;

public class DiscordTargetOptions
{
    public TimeSpan messageInterval = TimeSpan.FromSeconds(1);
    public TimeSpan editThreshold = TimeSpan.FromSeconds(10);
    public uint maxCacheSize = 50;
}

public class DiscordTarget :TargetWithLayout
{
    private readonly Logger logger = LogManager.GetCurrentClassLogger();
    private DiscordChannel logChannel;
    private DiscordTargetOptions options;
    private readonly ConcurrentBag<LogEventInfo> cache;
    private readonly object @lock = new();

    private DiscordMessage lastMessage;
    private DateTime lastMessageTime = DateTime.MinValue;

    // Constructor for direct object instantiation
    public DiscordTarget(DiscordChannel logChannel, IEnumerable<LogEventInfo>? cache = null, DiscordTargetOptions options = null)
    {
        this.logChannel = logChannel ?? throw new ArgumentNullException(nameof(logChannel));
        this.options = options ?? new DiscordTargetOptions();
        this.cache = new ( cache ?? []) ;
    }

    // Constructor for using IOptionsMonitor<T> for configuration
    public DiscordTarget(IOptionsMonitor<DiscordChannel> logChannelOptions, IEnumerable<LogEventInfo>? cache, IOptionsMonitor<DiscordTargetOptions> optionsMonitor)
    {
        this.logChannel = logChannelOptions.CurrentValue ?? throw new ArgumentNullException(nameof(logChannelOptions));
        this.options = optionsMonitor.CurrentValue ?? new DiscordTargetOptions();
        this.cache = new(cache ?? []);

        // Listen for changes in options
        logChannelOptions.OnChange(newChannel => this.logChannel = newChannel);
        optionsMonitor.OnChange(newOptions => this.options = newOptions);
    }

    private bool CheckGuildMatches(LogEventInfo logEvent)
    {
        var hasGuild =logEvent.Properties.TryGetValue("GuildId", out var guildId);
        if(!hasGuild)
            return false;

        return guildId!.ToString() == logChannel.GuildId!.Value.ToString();
    }

    protected override void Write(LogEventInfo logEvent)
    {
        if(!CheckGuildMatches(logEvent))
            return;

        cache.Add(logEvent);

        if(cache.Count >= options.maxCacheSize)
        {
            FlushCache();
        }

        if((DateTime.Now - lastMessageTime) < options.messageInterval)
            return;

        try
        {
            string logMessage;
            lock(@lock)
            {
                logMessage = string.Join(Environment.NewLine, cache.Select(e => this.Layout.Render(e)));
                WriteToChannel(logMessage).Wait();
                cache.Clear();
            }

        } catch(Exception ex)
        {
            logger.Error($"Error sending log message to Discord: {ex.Message}");
        }
    }

    private async Task WriteToChannel(string message)
    {
        if((DateTime.Now - lastMessageTime) < options.editThreshold)
        {
            await lastMessage.ModifyAsync(lastMessage.Content + Environment.NewLine + message);
        } else
        {
            lastMessage = await logChannel.SendMessageAsync(message);
            lastMessageTime = DateTime.Now;
        }
    }

    private void FlushCache()
    {
        lock(@lock)
        {
            if(cache.IsEmpty)
                return;

            try
            {
                string logMessage = string.Join(Environment.NewLine, cache.Select(e => this.Layout.Render(e)));
                WriteToChannel(logMessage).Wait();
                cache.Clear();
            } catch(Exception ex)
            {
                logger.Error($"Error sending log message to Discord: {ex.Message}");
            }
        }
    }
}
