using System;
using System.Linq;

namespace Doorfail.Core.Utils.Extensions
{
    public enum DateTimeIntervalType
    {
        Century = 0, // Interval type is in century.
        Decade = 1, // Interval type is in decades.
        Year = 2, // Interval type is in years.
        Month = 3, // Interval type is in months.
        Weeks = 4, // Interval type is in weeks.
        Day = 5, // Interval type is in days.
        Hour = 6, // Interval type is in hours.
        Minute = 7, // Interval type is in minutes.
        Second = 8, // Interval type is in seconds.
        Millisecond = 9, // Interval type is in milliseconds.
        Tick = 10 // Interval type is in ticks.
    }

    public static class DateTimeExtensions
    {
        public static string FormatTimeSpan(this TimeSpan timeSpan, string pasttenseFormat = "{0} {1} ago", string futuretenseFormat = "in {0} {1}", string currentTense = "now", DateTimeIntervalType minIntervalSize = DateTimeIntervalType.Minute)
        {
            bool isPast = timeSpan.Ticks < 0;
            if(isPast)
                timeSpan = timeSpan.Negate();

            int numericalValue;
            var timeValue = new (DateTimeIntervalType type, double threshold, Func<TimeSpan, int> converter)[]
            {
                (DateTimeIntervalType.Century, 365.25 * 100, ts => (int)(ts.TotalDays / (365.25 * 100))),
                (DateTimeIntervalType.Decade, 365.25 * 10, ts => (int)(ts.TotalDays / (365.25 * 10))),
                (DateTimeIntervalType.Year, 365.25, ts => (int)(ts.TotalDays / 365.25)),
                (DateTimeIntervalType.Month, 30, ts => (int)(ts.TotalDays / 30)),
                (DateTimeIntervalType.Weeks, 7, ts => (int)(ts.TotalDays / 7)),
                (DateTimeIntervalType.Day, 1, ts => (int)ts.TotalDays),
                (DateTimeIntervalType.Hour, 1, ts => (int)ts.TotalHours),
                (DateTimeIntervalType.Minute, 1, ts => (int)ts.TotalMinutes),
                (DateTimeIntervalType.Second, 1, ts => (int)ts.TotalSeconds),
                (DateTimeIntervalType.Millisecond, 1, ts => (int)ts.TotalMilliseconds),
                (DateTimeIntervalType.Tick, 0, ts => (int)ts.Ticks)
            }
            .Where(w => w.type <= minIntervalSize)
            .OrderBy(o => o.type)
            .First(t => t.converter(timeSpan) >= t.threshold);

            return timeValue.converter(timeSpan) == 0 ? currentTense
                : string.Format(isPast ? pasttenseFormat : futuretenseFormat,
                numericalValue = timeValue.converter(timeSpan),
                timeValue.type.ToString().ToLower() + (numericalValue > 1 ? "s" : ""));
        }
    }
}
