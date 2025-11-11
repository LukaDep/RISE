namespace Rise.Domain.Campus;

public class Building : Entity
{
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _address = string.Empty;
    public required string Address
    {
        get => _address;
        set => _address = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _type = string.Empty;
    public required string Type
    {
        get => _type;
        set => _type = Guard.Against.NullOrWhiteSpace(value);
    }

    private double? _latitude;
    public double? Latitude
    {
        get => _latitude;
        set => _latitude = value;
    }

    private double? _longitude;
    public double? Longitude
    {
        get => _longitude;
        set => _longitude = value;
    }

    private Guid _campusId;

    public required Guid CampusId
    {
        get => _campusId;
        set => _campusId = Guard.Against.Default(value);
    }

    private string _buildingCode = string.Empty;
    public required string BuildingCode
    {
        get => _buildingCode;
        set => _buildingCode = Guard.Against.NullOrWhiteSpace(value);
    }
}
