using System;

namespace Zombies.Domain.BuildingBocks
{
    /// <summary>
    /// represents the system clock
    /// </summary>
    public interface IClock
    {
        DateTime Now { get; }

        DateTime UtcNow { get; }

        DateTime GetLocalTime(DateTime utcDateTime);

        DateTime GetTimeForTimeZone(DateTime utcDateTime, TimeZoneInfo TimeZone);
    }

    /// <summary>
    /// represents the system clock
    /// usage: inherit this class to implement your custom system clock or invoke static <see cref="GetClock"/> method to get a actual system clock instance
    /// </summary>
    public abstract class Clock : IClock
    {
        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;

        public static IClock GetClock()
        {
            return new SystemClock();
        }

        public DateTime GetLocalTime(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.Local);
        }

        public DateTime GetTimeForTimeZone(DateTime utcDateTime, TimeZoneInfo TimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZone);
        }

        private class SystemClock : Clock { }
    }
}