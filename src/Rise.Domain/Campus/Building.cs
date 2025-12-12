namespace Rise.Domain.Campus;

/// <summary>
/// Represents a building within a campus.
/// Contains building details including name, address, type, location coordinates, and building code.
/// </summary>
public class Building : Entity
{
    /// <summary>
    /// The name of the building.
    /// </summary>
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The street address of the building.
    /// </summary>
    private string _address = string.Empty;
    public required string Address
    {
        get => _address;
        set => _address = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The type or category of the building (e.g., academic, administrative, residential).
    /// </summary>
    private string _type = string.Empty;
    public required string Type
    {
        get => _type;
        set => _type = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The latitude coordinate of the building location.
    /// </summary>
    private double? _latitude;
    public double? Latitude
    {
        get => _latitude;
        set => _latitude = value;
    }

    /// <summary>
    /// The longitude coordinate of the building location.
    /// </summary>
    private double? _longitude;
    public double? Longitude
    {
        get => _longitude;
        set => _longitude = value;
    }

    /// <summary>
    /// The unique identifier of the campus this building belongs to.
    /// </summary>
    private Guid _campusId;

    public required Guid CampusId
    {
        get => _campusId;
        set => _campusId = Guard.Against.Default(value);
    }

    /// <summary>
    /// The unique building code identifier (e.g., "B1", "A2").
    /// </summary>
    private string _buildingCode = string.Empty;
    public required string BuildingCode
    {
        get => _buildingCode;
        set => _buildingCode = Guard.Against.NullOrWhiteSpace(value);
    }
}
