namespace Rise.Shared.Campus;

public class BuildingDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public int? X { get; set; } // pixel, for image plans
    public int? Y { get; set; }
    public required string Type { get; set; }
}