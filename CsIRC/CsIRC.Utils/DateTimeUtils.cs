using System;

namespace CsIRC.Utils
{
    /// <summary>
    /// Utility class for working with dates and times.
    /// </summary>
    public static class DateTimeUtils
    {
        /// <summary>
        /// Converts a Unix timestamp to a DateTime.
        /// </summary>
        /// <param name="unixTimestamp">The unix timestamp.</param>
        /// <returns>The resulting date and time.</returns>
        public static DateTime FromUnixTime(long unixTimestamp)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimestamp);
        }
    }
}
