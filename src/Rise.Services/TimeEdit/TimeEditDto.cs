namespace Rise.Shared.TimeEdit;

/// <summary>
/// Contains data transfer objects (DTOs) used for timeedit data retrieval.
/// </summary>
public static class TimeEditDto
{
    /// <summary>
    /// Represents a response from the timeedit api.
    /// </summary>
    // Root object
    public class ApiResponse
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