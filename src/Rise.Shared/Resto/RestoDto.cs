namespace Rise.Shared.Resto;

/// <summary>
/// Data transfer objects for restaurant information.
/// </summary>
public static class RestoDto
{
    /// <summary>
    /// Represents a restaurant for display and retrieval.
    /// Contains details including name, description, location, opening hours, kitchen types, and contact information.
    /// Includes computed properties for current open status and upcoming opening/closing times.
    /// </summary>
    public class Index
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required Guid BuildingId { get; set; }
        public Dictionary<DayOfWeek, string>? OpeningHours { get; set; }
        public bool IsCurrentlyOpen { get; set; }
        public DateTime? NextOpeningTime { get; set; }
        public DateTime? NextClosingTime { get; set; }
        public List<string>? KitchenType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? ImageUrl { get; set; }
    }
}