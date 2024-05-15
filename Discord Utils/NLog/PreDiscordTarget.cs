using System.Collections.Concurrent;
using NLog;
using NLog.Targets;

namespace Doorfail.Core.NLog;
public class PreDiscordTarget :TargetWithLayout
{
    private ConcurrentBag<LogEventInfo> _cachedLogEvents = new ConcurrentBag<LogEventInfo>();

    protected override void Write(LogEventInfo logEvent) => _cachedLogEvents.Add(logEvent);

    public IEnumerable<LogEventInfo> GetCachedLogEvents() => _cachedLogEvents.ToArray();
}