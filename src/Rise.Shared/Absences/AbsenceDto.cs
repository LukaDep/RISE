namespace Rise.Shared.Absences;

/// <summary>
/// Data transfer objects for teacher absences.
/// </summary>
public static class AbsenceDto
{
    /// <summary>
    /// Represents an absence record for display and retrieval.
    /// Contains teacher name, absence period, and reason.
    /// </summary>
    public class Index
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string Reason { get; set; }
    }
}