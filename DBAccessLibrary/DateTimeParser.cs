using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccessLibrary
{
    public class DateTimeParser
    {
        public static string GetNotificationSentTime(DateTime dateTimeUtc)
        {
            DateTime utcStamp = dateTimeUtc.Kind == DateTimeKind.Utc
                ? dateTimeUtc
                : DateTime.SpecifyKind(dateTimeUtc, DateTimeKind.Utc);

            TimeSpan timeSpan = DateTime.UtcNow - utcStamp;
            if (timeSpan < TimeSpan.Zero || timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalHours < 1)
                return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes == 1 ? "" : "s")} ago";
            if (timeSpan.TotalDays < 1)
                return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours == 1 ? "" : "s")} ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays == 1 ? "" : "s")} ago";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) == 1 ? "" : "s")} ago";
            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) == 1 ? "" : "s")} ago";
        }
    }
}
