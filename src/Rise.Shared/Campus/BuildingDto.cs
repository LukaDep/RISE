namespace Rise.Shared.Campus;

/// <summary>
/// Data transfer objects for building information.
/// </summary>
public class BuildingDto
{
    /// <summary>
    /// Represents a building for display and retrieval.
    /// Contains building details including name, address, type, building code, and location coordinates.
    /// </summary>
    public class Index
    {
        public required Guid Id { get; set; }
        public required Guid CampusId { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string Type { get; set; }
        public required string BuildingCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}