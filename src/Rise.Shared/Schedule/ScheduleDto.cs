namespace Rise.Shared.Schedule;

/// <summary>
/// Contains data transfer objects (DTOs) used for schedule data retrieval.
/// </summary>
public static class ScheduleDto
{

    /// <summary>
    /// Represents the API response containing a list of schedules.
    /// </summary>
    public class Data
    {
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }

    // Schedule object
    public class Schedule
    {
        public string Id { get; set; } = default!;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Course { get; set; } = default!;
        public string WorkForm { get; set; } = default!;
        public string Environment { get; set; } = default!;
        public string Room { get; set; } = default!;
        public string Teacher { get; set; } = default!;
        public bool IsAbsent { get; set; } = false;
    }
}