namespace Rise.Shared.Resto;

public static class RestoDto
{
    public class Index
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string BuildingId { get; set; }
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