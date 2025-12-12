namespace Rise.Shared.Schedule;

/// <summary>
/// Response wrappers for external schedule API data.
/// Contains structures for deserializing raw schedule data from the external API.
/// </summary>
public static class ScheduleApiResponse
{
    /// <summary>
    /// Root response object from the external schedule API.
    /// Contains column headers, metadata information, and the list of schedule entries.
    /// </summary>
    public class ScheduleData
    {
        public string Email { get; set; } = default!;
        public List<string> ColumnHeaders { get; set; } = new();
        public Info Info { get; set; } = new();
        public List<Schedule> Schedules { get; set; } = new();
    }

    /// <summary>
    /// Metadata information about the schedule response.
    /// Includes limits and counts for pagination and data handling.
    /// </summary>
    public class Info
    {
        public int ScheduleLimit { get; set; }
        public int ScheduleCount { get; set; }
    }

    /// <summary>
    /// Individual schedule entry from the external API.
    /// Contains raw schedule data including ID, date/time information, and column data that needs to be parsed.
    /// </summary>
    public class Schedule
    {
        public string Id { get; set; } = default!;
        public string StartDate { get; set; } = default!;
        public string StartTime { get; set; } = default!;
        public string EndDate { get; set; } = default!;
        public string EndTime { get; set; } = default!;
        public List<string> Columns { get; set; } = new();
    }
}