using System.Globalization;
using System.Net;
using DisCatSharp.Attributes;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using Microsoft.Extensions.Logging;

namespace Doorfail.Core;

public class DiscordConfig
{
    /// <summary>
    /// Sets the token used to identify the client.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// <para>Sets the type of the token used to identify the client.</para>
    /// <para>Defaults to <see cref="TokenType.Bot"/>.</para>
    /// </summary>
    public TokenType TokenType { get; private set; } = TokenType.Bot;

    /// <summary>
    /// <para>Sets the minimum logging level for messages.</para>
    /// <para>Defaults to <see cref="LogLevel.Information"/>.</para>
    /// </summary>
    public LogLevel MinimumLogLevel { internal get; set; } = LogLevel.Information;

    /// <summary>
    /// Overwrites the api version.
    /// Defaults to 10.
    /// </summary>
    public string ApiVersion { internal get; set; } = "10";

    /// <summary>
    /// <para>Sets whether to rely on Discord for NTP (Network Time Protocol) synchronization with the "X-Ratelimit-Reset-After" header.</para>
    /// <para>If the system clock is unsynced, setting this to true will ensure ratelimits are synced with Discord and reduce the risk of hitting one.</para>
    /// <para>This should only be set to false if the system clock is synced with NTP.</para>
    /// <para>Defaults to <see langword="true"/>.</para>
    /// </summary>
    public bool UseRelativeRatelimit { internal get; set; } = true;

    /// <summary>
    /// <para>Allows you to overwrite the time format used by the internal debug logger.</para>
    /// <para>Only applicable when <see cref="LoggerFactory"/> is set left at default value. Defaults to ISO 8601-like format.</para>
    /// </summary>
    public string LogTimestampFormat { internal get; set; } = "yyyy-MM-dd HH:mm:ss zzz";

    /// <summary>
    /// <para>Sets the member count threshold at which guilds are considered large.</para>
    /// <para>Defaults to 250.</para>
    /// </summary>
    public int LargeThreshold { internal get; set; } = 250;

    /// <summary>
    /// <para>Sets whether to automatically reconnect in case a connection is lost.</para>
    /// <para>Defaults to <see langword="true"/>.</para>
    /// </summary>
    public bool AutoReconnect { internal get; set; } = true;

    /// <summary>
    /// <para>Sets the ID of the shard to connect to.</para>
    /// <para>If not sharding, or sharding automatically, this value should be left with the default value of 0.</para>
    /// </summary>
    public int ShardId { internal get; set; } = 0;

    /// <summary>
    /// <para>Sets the total number of shards the bot is on. If not sharding, this value should be left with a default value of 1.</para>
    /// <para>If sharding automatically, this value will indicate how many shards to boot. If left default for automatic sharding, the client will determine the shard count automatically.</para>
    /// </summary>
    public int ShardCount { internal get; set; } = 1;

    /// <summary>
    /// <para>Sets the level of compression for WebSocket traffic.</para>
    /// <para>Disabling this option will increase the amount of traffic sent via WebSocket. Setting <see cref="GatewayCompressionLevel.Payload"/> will enable compression for READY and GUILD_CREATE payloads. Setting <see cref="GatewayCompressionLevel.Stream"/> will enable compression for the entire WebSocket stream, drastically reducing amount of traffic.</para>
    /// <para>Defaults to <see cref="GatewayCompressionLevel.Stream"/>.</para>
    /// </summary>
    public GatewayCompressionLevel GatewayCompressionLevel { internal get; set; } = GatewayCompressionLevel.Stream;

    /// <summary>
    /// <para>Sets the size of the global message cache.</para>
    /// <para>Setting this to 0 will disable message caching entirely.</para>
    /// <para>Defaults to 1024.</para>
    /// </summary>
    public int MessageCacheSize { internal get; set; } = 1024;

    /// <summary>
    /// <para>Sets the proxy to use for HTTP and WebSocket connections to Discord.</para>
    /// <para>Defaults to <see langword="null"/>.</para>
    /// </summary>
    public IWebProxy Proxy { internal get; set; } = null;

    /// <summary>
    /// <para>Sets the timeout for HTTP requests.</para>
    /// <para>Set to <see cref="Timeout.InfiniteTimeSpan"/> to disable timeouts.</para>
    /// <para>Defaults to 20 seconds.</para>
    /// </summary>
    public TimeSpan HttpTimeout { internal get; set; } = TimeSpan.FromSeconds(20);

    /// <summary>
    /// <para>Defines that the client should attempt to reconnect indefinitely.</para>
    /// <para>This is typically a very bad idea to set to <c>true</c>, as it will swallow all connection errors.</para>
    /// <para>Defaults to <see langword="false"/>.</para>
    /// </summary>
    public bool ReconnectIndefinitely { internal get; set; } = false;

    /// <summary>
    /// Sets whether the client should attempt to cache members if exclusively using unprivileged intents.
    /// <para>
    ///     This will only take effect if there are no <see cref="DiscordIntents.GuildMembers"/> or <see cref="DiscordIntents.GuildPresences"/>
    ///     intents specified. Otherwise, this will always be overwritten to true.
    /// </para>
    /// <para>Defaults to <see langword="true"/>.</para>
    /// </summary>
    public bool AlwaysCacheMembers { internal get; set; } = true;

    /// <summary>
    /// Sets whether a shard logger is attached.
    /// </summary>
    internal bool HasShardLogger { get; set; } = false;

    /// <summary>
    /// <para>Sets the gateway intents for this client.</para>
    /// <para>If set, the client will only receive events that they specify with intents.</para>
    /// <para>Defaults to <see cref="DiscordIntents.AllUnprivileged"/>.</para>
    /// </summary>
    public DiscordIntents Intents { internal get; set; } = DiscordIntents.AllUnprivileged;

    /// <summary>
    /// <para>Sets the logger implementation to use.</para>
    /// <para>To create your own logger, implement the <see cref="Microsoft.Extensions.Logging.ILoggerFactory"/> instance.</para>
    /// <para>Defaults to built-in implementation.</para>
    /// </summary>
    public ILoggerFactory LoggerFactory { internal get; set; } = null!;

    /// <summary>
    /// <para>Sets if the bot's status should show the mobile icon.</para>
    /// <para>Defaults to <see langword="false"/>.</para>
    /// </summary>
    public bool MobileStatus { internal get; set; } = false;

    /// <summary>
    /// <para>Which api channel to use.</para>
    /// <para>Defaults to <see cref="ApiChannel.Stable"/>.</para>
    /// </summary>
    public ApiChannel ApiChannel { get; private set; } = ApiChannel.Stable;

    /// <summary>
    /// <para>Refresh full guild channel cache.</para>
    /// <para>Defaults to <see langword="false"/>.</para>
    /// </summary>
    public bool AutoRefreshChannelCache { internal get; set; } = false;

    /// <summary>
    /// Sets your preferred API language. See <see cref="DiscordLocales" /> for valid locales.
    /// </summary>
    public string Locale { internal get; set; } = CultureInfo.CurrentCulture.Name;

    /// <summary>
    /// Sets your timezone.
    /// </summary>
    public string Timezone { internal get; set; } = null;

    /// <summary>
    /// <para>Whether to attach the bots username and id to sentry reports.</para>
    /// <para>This helps us to pinpoint problems.</para>
    /// <para>Defaults to <see langword="false"/>.</para>
    /// </summary>
    public bool AttachUserInfo { get; private set; } = true;

    /// <summary>
    /// <para>Your email address we can reach out when your bot encounters library bugs.</para>
    /// <para>Will only be transmitted if <see cref="AttachUserInfo"/> is <see langword="true"/>.</para>
    /// <para>Defaults to <see langword="null"/>.</para>
    /// </summary>
    public string FeedbackEmail { internal get; set; } = "dev@doorfail.com";

    /// <summary>
    /// <para>Your discord user id we can reach out when your bot encounters library bugs.</para>
    /// <para>Will only be transmitted if <see cref="AttachUserInfo"/> is <see langword="true"/>.</para>
    /// <para>Defaults to <see langword="null"/>.</para>
    /// </summary>
    public ulong? DeveloperUserId { get; private set; } = 264414780297707520;

    /// <summary>
    /// Whether to disable the exception filter.
    /// </summary>
    internal bool DisableExceptionFilter { get; set; } = false;

    /// <summary>
    /// Custom Sentry Dsn.
    /// </summary>
    internal string CustomSentryDsn { get; set; } = null;

    /// <summary>
    /// Whether to autofetch the sku ids.
    /// <para>Mutually exclusive to <see cref="SkuId"/> and <see cref="TestSkuId"/>.</para>
    /// </summary>
    [RequiresFeature(Features.MonetizedApplication)]
    public bool AutoFetchSkuIds { internal get; set; } = false;

    /// <summary>
    /// The applications sku id for premium apps.
    /// <para>Mutually exclusive to <see cref="AutoFetchSkuIds"/>.</para>
    /// </summary>
    [RequiresFeature(Features.MonetizedApplication)]
    public ulong? SkuId { internal get; set; } = null;

    /// <summary>
    /// The applications test sku id for premium apps.
    /// <para>Mutually exclusive to <see cref="AutoFetchSkuIds"/>.</para>
    /// </summary>
    [RequiresFeature(Features.MonetizedApplication)]
    public ulong? TestSkuId { internal get; set; } = null;

    /// <summary>
    /// Whether to disable the update check.
    /// </summary>
    public bool DisableUpdateCheck { internal get; set; } = false;

    /// <summary>
    /// Against which channel to check for updates.
    /// <para>Defaults to <see cref="VersionCheckMode.NuGet"/>.</para>
    /// </summary>
    public VersionCheckMode UpdateCheckMode { internal get; set; } = VersionCheckMode.NuGet;

    /// <summary>
    /// Whether to include prerelease versions in the update check.
    /// </summary>
    public bool IncludePrereleaseInUpdateCheck { get; internal set; } = true;

    /// <summary>
    /// Sets the GitHub token to use for the update check.
    /// <para>Only useful if extensions are private and <see cref="UpdateCheckMode"/> is <see cref="VersionCheckMode.GitHub"/>.</para>
    /// </summary>
    public string UpdateCheckGitHubToken { get; set; } = null;

    /// <summary>
    /// Whether to show release notes in the update check.
    /// <para>Defaults to <see langword="false"/>.</para>
    /// </summary>
    public bool ShowReleaseNotesInUpdateCheck { get; set; } = false;
    public ulong? DebugGuildId { get; set; }
}