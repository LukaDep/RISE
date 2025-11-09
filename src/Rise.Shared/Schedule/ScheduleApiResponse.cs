namespace Rise.Shared.Schedule;

/// <summary>
/// Contains data transfer objects (DTOs) used for schedule data retrieval.
/// </summary>
public static class ScheduleApiResponse
{
    /// <summary>
    /// Represents a response from the schedule api.
    /// </summary>
    // Root object
    public class ScheduleData
    {
        public List<string> ColumnHeaders { get; set; } = new();
        public Info Info { get; set; } = new();
        public List<Reservation> Reservations { get; set; } = new();
    }

    // Info object
    public class Info
    {
        public int ReservationLimit { get; set; }
        public int ReservationCount { get; set; }
    }

    // Reservation object
    public class Reservation
    {
        public string Id { get; set; } = default!;
        public string StartDate { get; set; } = default!;
        public string StartTime { get; set; } = default!;
        public string EndDate { get; set; } = default!;
        public string EndTime { get; set; } = default!;
        public List<string> Columns { get; set; } = new();
    }


}