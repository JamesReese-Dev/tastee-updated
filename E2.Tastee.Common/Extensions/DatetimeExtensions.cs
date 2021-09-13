using System;
using System.Runtime.InteropServices;

namespace E2.Tastee.Common.Extensions
{
    public static class DatetimeExtensions
    {
        private static TimeZoneInfo _defaultTimeZoneInfo = null;

        private static bool isDefaultTimezone(string tzString)
            => tzString == AppConstants.DEFAULT_TIME_ZONE;

        private static TimeZoneInfo resolveTimezoneInfo(string tzString)
        {
            if (isDefaultTimezone(tzString) && _defaultTimeZoneInfo != null)
            {
                return _defaultTimeZoneInfo;
            }
            TimeZoneInfo result = TimeZoneInfo.Utc;
            try
            {
                // TODO: in the event of more time zones being used we should look into the "TZConvert" nuget package
                // to enable usage of either Windows or Linux friendly time zone ids
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && isDefaultTimezone(tzString))
                {
                    result = TimeZoneInfo.FindSystemTimeZoneById(AppConstants.LINUX_DEFAULT_TIME_ZONE_STRING);
                }
                result = TimeZoneInfo.FindSystemTimeZoneById(tzString);
            }
            catch // (TimeZoneNotFoundException)
            {
            }
            if (isDefaultTimezone(tzString))
            {
                _defaultTimeZoneInfo = result;
            }
            return result;
        }

        public static DateTime Trim(this DateTime dateTime, long roundTicks)
            => new DateTime(dateTime.Ticks - dateTime.Ticks % roundTicks, dateTime.Kind);

        public static DateTime FromUtcToTimezone(this DateTime dateTime, string toTimeZone)
        {
            TimeZoneInfo tz = resolveTimezoneInfo(toTimeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), tz);
        }

        public static DateTime FromTimezoneToUtc(this DateTime dateTime, string fromTimeZone)
        {
            TimeZoneInfo tz = resolveTimezoneInfo(fromTimeZone);
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, tz);
        }
    }
}
