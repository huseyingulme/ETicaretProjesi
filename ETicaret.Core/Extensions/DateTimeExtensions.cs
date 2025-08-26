using System.Globalization;

namespace ETicaret.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToTurkishDateString(this DateTime dateTime)
        {
            var culture = new CultureInfo("tr-TR");
            return dateTime.ToString("dd MMMM yyyy", culture);
        }

        public static string ToTurkishDateTimeString(this DateTime dateTime)
        {
            var culture = new CultureInfo("tr-TR");
            return dateTime.ToString("dd MMMM yyyy HH:mm", culture);
        }

        public static string ToRelativeTimeString(this DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return "Az önce";
            
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} dakika önce";
            
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} saat önce";
            
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} gün önce";
            
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} hafta önce";
            
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} ay önce";
            
            return $"{(int)(timeSpan.TotalDays / 365)} yıl önce";
        }

        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        public static bool IsYesterday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(-1);
        }

        public static bool IsThisWeek(this DateTime dateTime)
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            return dateTime >= startOfWeek && dateTime < endOfWeek;
        }

        public static bool IsThisMonth(this DateTime dateTime)
        {
            return dateTime.Month == DateTime.Today.Month && dateTime.Year == DateTime.Today.Year;
        }

        public static bool IsThisYear(this DateTime dateTime)
        {
            return dateTime.Year == DateTime.Today.Year;
        }

        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime StartOfWeek(this DateTime dateTime)
        {
            var diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dateTime.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dateTime)
        {
            return dateTime.StartOfWeek().AddDays(6).EndOfDay();
        }

        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime dateTime)
        {
            return dateTime.StartOfMonth().AddMonths(1).AddDays(-1).EndOfDay();
        }

        public static DateTime StartOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        public static DateTime EndOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 12, 31).EndOfDay();
        }

        public static int GetAge(this DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public static bool IsWeekend(this DateTime dateTime)
        {
            return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
        }

        public static bool IsWeekday(this DateTime dateTime)
        {
            return !dateTime.IsWeekend();
        }

        public static DateTime AddBusinessDays(this DateTime dateTime, int businessDays)
        {
            var direction = businessDays < 0 ? -1 : 1;
            var absBusinessDays = Math.Abs(businessDays);
            var result = dateTime;

            while (absBusinessDays > 0)
            {
                result = result.AddDays(direction);
                if (result.IsWeekday())
                {
                    absBusinessDays--;
                }
            }

            return result;
        }

        public static int GetBusinessDaysBetween(this DateTime startDate, DateTime endDate)
        {
            var businessDays = 0;
            var currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                if (currentDate.IsWeekday())
                {
                    businessDays++;
                }
                currentDate = currentDate.AddDays(1);
            }

            return businessDays;
        }

        public static string ToISO8601String(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
        }

        public static DateTime ToTurkishTimeZone(this DateTime utcDateTime)
        {
            var turkishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, turkishTimeZone);
        }

        public static DateTime ToUtcFromTurkish(this DateTime turkishDateTime)
        {
            var turkishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            return TimeZoneInfo.ConvertTimeToUtc(turkishDateTime, turkishTimeZone);
        }

        public static bool IsBetween(this DateTime dateTime, DateTime startDate, DateTime endDate)
        {
            return dateTime >= startDate && dateTime <= endDate;
        }

        public static TimeSpan GetTimeUntil(this DateTime dateTime)
        {
            return dateTime - DateTime.UtcNow;
        }

        public static TimeSpan GetTimeSince(this DateTime dateTime)
        {
            return DateTime.UtcNow - dateTime;
        }

        public static string ToShortTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }

        public static string ToLongTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        public static string ToShortDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");
        }

        public static string ToLongDateString(this DateTime dateTime)
        {
            var culture = new CultureInfo("tr-TR");
            return dateTime.ToString("dd MMMM yyyy dddd", culture);
        }
    }
}
