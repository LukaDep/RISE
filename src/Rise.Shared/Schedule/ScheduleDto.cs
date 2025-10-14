namespace Rise.Shared.Schedule;

/// <summary>
/// Contains data transfer objects (DTOs) used for schedule data retrieval.
/// </summary>
public static class ScheduleDto
{

    /// <summary>
    /// Represents the API response containing a list of reservations.
    /// </summary>
    public class Data
    {
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

    // Reservation object
    public class Reservation
    {
        public string Id { get; set; } = default!;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Course { get; set; } = default!;
        public string WorkForm { get; set; } = default!;
        public string Environment { get; set; } = default!;
        public string Room { get; set; } = default!;
        public string Teacher { get; set; } = default!;
    }
}