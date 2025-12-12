namespace Rise.Shared.Campus;

/// <summary>
/// Data transfer objects for campus information.
/// </summary>
public static class CampusDto
{
    /// <summary>
    /// Represents a campus for display and retrieval.
    /// Contains address, contact information, facilities, location coordinates, and associated buildings.
    /// </summary>
    public class Index
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Street { get; set; }
        public required string HouseNumber { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string ContactPhone { get; set; }
        public required string Description { get; set; }
        public required IEnumerable<string> Facilities { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public IEnumerable<BuildingDto.Index>? Buildings { get; set; }
    }
}