namespace Rise.Client.Schedule;

/// <summary>
/// Utility class with helper methods for schedule views
/// </summary>
public static class ScheduleHelpers
{
    /// <summary>
    /// Truncates a title to a maximum length and adds ellipsis if needed
    /// </summary>
    public static string TruncateTitle(string title, int maxLength = 40)
    {
        if (string.IsNullOrEmpty(title))
            return title;

        if (title.Length <= maxLength)
            return title;

        return title.Substring(0, maxLength) + "...";
    }

    /// <summary>
    /// Gets the background color class for a specific event type
    /// </summary>
    public static string GetEventTypeBgColor(string type) => type.ToLower() switch
    {
        "hoorcollege" => "bg-hogent-education-30 text-hogent-education",
        "activerend hoorcollege" => "bg-hogent-it-30 text-hogent-it",
        "practicum" => "bg-hogent-green-30 text-hogent-green",
        "werkcollege" => "bg-hogent-orange-30 text-hogent-orange",
        "seminarie" => "bg-hogent-business-30 text-hogent-business",
        _ => "bg-hogent-black-30 text-hogent-black"
    };

    /// <summary>
    /// Gets the border color class for a specific event type
    /// </summary>
    public static string GetEventTypeBorderColor(string type) => type.ToLower() switch
    {
        "hoorcollege" => "border-hogent-education-30 text-hogent-education",
        "activerend hoorcollege" => "border-hogent-it-30 text-hogent-it",
        "practicum" => "border-hogent-green-30 text-hogent-green",
        "werkcollege" => "border-hogent-orange-30 text-hogent-orange",
        "seminarie" => "border-hogent-business-30 text-hogent-business",
        _ => "border-hogent-black-30 text-hogent-black"
    };

    /// <summary>
    /// Calculates the position (in pixels) of the current time on the schedule grid
    /// Assumes 8:00 AM start time and 64px per hour
    /// </summary>
    public static double GetCurrentTimePosition(DateTime currentTime, int startHour = 8, int pixelsPerHour = 64)
    {
        var hour = currentTime.Hour;
        var minute = currentTime.Minute;

        if (hour < startHour || hour > 20)
            return -1;

        return ((hour - startHour) * pixelsPerHour) + (minute * pixelsPerHour / 60.0);
    }

    /// <summary>
    /// Calculates the top position for an event based on its start time
    /// </summary>
    public static int CalculateEventTopPosition(DateTime startDateTime, int startHour = 8, int pixelsPerHour = 64)
    {
        var startHourValue = startDateTime.Hour;
        var startMinute = startDateTime.Minute;
        return ((startHourValue - startHour) * pixelsPerHour) + ((startMinute * pixelsPerHour) / 60);
    }

    /// <summary>
    /// Calculates the height for an event based on its duration
    /// </summary>
    public static int CalculateEventHeight(DateTime startDateTime, DateTime endDateTime, int pixelsPerHour = 64)
    {
        var startHour = startDateTime.Hour;
        var startMinute = startDateTime.Minute;
        var endHour = endDateTime.Hour;
        var endMinute = endDateTime.Minute;

        var durationMinutes = (endHour - startHour) * 60 + (endMinute - startMinute);
        return (durationMinutes * pixelsPerHour) / 60;
    }

    /// <summary>
    /// Gets the duration in minutes between two DateTimes
    /// </summary>
    public static int GetDurationMinutes(DateTime startDateTime, DateTime endDateTime)
    {
        var startHour = startDateTime.Hour;
        var startMinute = startDateTime.Minute;
        var endHour = endDateTime.Hour;
        var endMinute = endDateTime.Minute;

        return (endHour - startHour) * 60 + (endMinute - startMinute);
    }

    /// <summary>
    /// Gets the start of the week (Monday) for a given date
    /// </summary>
    public static DateTime GetWeekStart(DateTime date)
    {
        var dayOfWeek = (int)date.DayOfWeek;
        var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
        return date.AddDays(-daysToSubtract);
    }

    /// <summary>
    /// Gets the week number for a given date
    /// </summary>
    public static int GetWeekNumber(DateTime date) =>
        System.Globalization.ISOWeek.GetWeekOfYear(date);

    /// <summary>
    /// Gets the list of weekdays for a given date's week
    /// </summary>
    /// <param name="date">The date to get the week days for</param>
    /// <param name="includeWeekend">If true, includes Saturday and Sunday; if false, returns only Monday-Friday</param>
    public static List<DateTime> GetWeekDays(DateTime date, bool includeWeekend = false)
    {
        var weekStart = GetWeekStart(date);
        var daysToInclude = includeWeekend ? 7 : 5;
        return Enumerable.Range(0, daysToInclude)
                         .Select(i => weekStart.AddDays(i))
                         .ToList();
    }

    /// <summary>
    /// Moves to the previous weekday (skipping weekends)
    /// </summary>
    public static DateTime PreviousWeekday(DateTime date)
    {
        do { date = date.AddDays(-1); }
        while (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
        return date;
    }

    /// <summary>
    /// Moves to the next weekday (skipping weekends)
    /// </summary>
    public static DateTime NextWeekday(DateTime date)
    {
        do { date = date.AddDays(1); }
        while (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
        return date;
    }
}
