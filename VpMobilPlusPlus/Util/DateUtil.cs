using System;
using System.Globalization;

namespace VpMobilPlusPlus.Util;

public class DateUtil
{
    public static DateTime GermanStringToDateTime(string date)
    {
        if (DateTime.TryParseExact(
                date,
                "dd.MM.yyyy, HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dt))
        {
            return dt;
        }
        return DateTime.MinValue;
    }

    public static DateTime GermanWeekDayStringToDateTime(string date)
    {
        CultureInfo culture = new CultureInfo("de-DE");
        DateTime dt = DateTime.ParseExact(date, "dddd, dd. MMMM yyyy", culture);
        return dt;
    }

    public static DateOnly GetCurrentWeeksMonday()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        
        if (today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday)
        {
            today = today.AddDays(7);
        }

        // ISO 8601 style: Monday = 1, Sunday = 7
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;

        DateOnly monday = today.AddDays(-diff);
        return monday;
    }

    public static DateOnly GetDatesMonday(DateOnly date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;

        DateOnly monday = date.AddDays(-diff);
        return monday;
    }
}