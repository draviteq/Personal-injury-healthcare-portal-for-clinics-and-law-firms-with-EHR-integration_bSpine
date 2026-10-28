using System.Globalization;

namespace CapitalBraineAndSpine.Service.Infrastructures.ExtensionHelper
{
    public static class DateTimeExtensions
    {
        public static string ToShortDate(this DateTime date)
        {
            return date.ToString("d, MMM yyyy");
        }

        public static string ToDateWithAge(this DateTime date)
        {
            return $"{date.ToString("D, MMM YYYY")}";
        }
        public static string ToShortDate(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("MMMM d, yyyy") : string.Empty;
        }
        public static string ToShortDateWithMonthName(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("MMM d, yyyy") : string.Empty;
        }
        public static DateTime LocalNow(this DateTime date)
        {
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTime(date, estZone);
        }
        public static string ToDateWithTime(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("MM/dd/yyyy hh:mm tt") : string.Empty;
        }
        public static string ToDateWithTime(this DateTime date)
        {
            return date.ToString("dd MMM yyyy hh:mm tt");
        }
        public static string ToDateWithDDMMYYYY(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }
        public static string ToDateWithMMDDYYYY(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }
        public static string ToDateWithYYYYMMDD(this DateTime date)
        {
            return date.ToString("MM-DD-yyyy");
        }
        public static string ToHourAndMintue(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("h:mm") : string.Empty;
        }
        public static string ToHourAndMintueTime(this DateTime date)
        {
            return $"{date.ToString("hh")}:{date.ToString("mm")}";
        }
        public static string ToDay(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("dd") : string.Empty;
        }
        public static string ToMonthName(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("MMMM") : string.Empty;
        }
        public static string ToYear(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("yyyy") : string.Empty;
        }
        public static DateTime RoundUpToNearest30(this DateTime datetime)
        {
            double atMinuteInBlock = datetime.TimeOfDay.TotalMinutes % 30;
            double minutesToAdd = 30 - atMinuteInBlock;
            return datetime.AddMinutes(minutesToAdd);
        }
        public static string FormatTimeWithAMPM(this string time)
        {
            // Parse the time string to a DateTime object (without specifying the date)
            if (DateTime.TryParseExact(time, "HH:mm", null, DateTimeStyles.None, out DateTime parsedTime))
            {
                // Format the time with AM/PM and return it
                return parsedTime.ToString("hh:mm tt");
            }

            // If parsing fails, return the input string as is
            return time;
        }
        public static string Change24HoursFormat(this string time)
        {
            // Parse the time string to a DateTime object (without specifying the date)
            if (DateTime.TryParseExact(time, "h:mm tt", null, DateTimeStyles.None, out DateTime parsedTime))
            {
                // Format the time with AM/PM and return it
                return parsedTime.ToString("HH:mm");
            }

            // If parsing fails, return the input string as is
            return time;
        }

        public static string GetDayNumberSuffix(this DateTime? date)
        {
            string day = date.HasValue ? date.Value.ToString("dd") : string.Empty;
            string suffix = string.Empty;

            if (!string.IsNullOrEmpty(day))
            {
                suffix = "th";
                if (int.Parse(day) < 11 || int.Parse(day) > 20)
                {

                    day = day.ToCharArray()[^1].ToString();
                    switch (day)
                    {
                        case "1":
                            suffix = "st";
                            break;
                        case "2":
                            suffix = "nd";
                            break;
                        case "3":
                            suffix = "rd";
                            break;
                    }
                }
            }
            return suffix;
        }
    }
}

