using TestTask.Models;

namespace TestTask.Extensions;

public static class DateTimeStringExtensions
{
    public static bool IsValidDateTimeString(this string value)
    {
        return DateTime.TryParse(value, out _);
    }

    public static DateTime ToDateTime(this string value)
    {
        return DateTime.Parse(value, default, System.Globalization.DateTimeStyles.AdjustToUniversal & System.Globalization.DateTimeStyles.AssumeUniversal);
    }

    public static bool IsValidFilterDateTimeString(this string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length < 6)
        {
            return false;
        }

        var prefixValid = FilterQuerry.IsValidPrefix(value[0..2]);

        return prefixValid && IsValidDateTimeString(value[2..]);
    }

    public static FilterQuerry ToFilterQuerry(this string value)
    {
        return new FilterQuerry(DateTime.Parse(value[2..]), value[..2]);
    }
}