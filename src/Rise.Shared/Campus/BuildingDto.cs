namespace Rise.Shared.Campus;

public class BuildingDto
{
    public class Index
    {
        public required Guid Id { get; set; }
        public required Guid CampusId { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // public List<ClassroomDto>? ClassRooms { get; set; }
    }
}