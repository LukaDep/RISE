namespace Rise.Shared.Campus;

public static class CampusDto
{

    public class Index
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Street { get; set; }
        public required string HouseNumber { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public string? MapImageUrl { get; set; } // null if no image
        public int? ImageWidth { get; set; }
        public int? ImageHeight { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<BuildingDto.Index>? Buildings { get; set; }
    }

}