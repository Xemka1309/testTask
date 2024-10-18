namespace TestTask.Extensions;

public static class DateTimeStringExtensions
{
    public static bool IsValidDateTimeString(this string value)
    {
        return DateTime.TryParse(value, out DateTime dateTime);
    }

    public static DateTime ToDateTime(this string value)
    {
        return DateTime.Parse(value, default, System.Globalization.DateTimeStyles.AdjustToUniversal & System.Globalization.DateTimeStyles.AssumeUniversal);
    }
}