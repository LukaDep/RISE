namespace Rise.Domain.Campus;

public class Campus : Entity
{
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _street = string.Empty;
    public required string Street
    {
        get => _street;
        set => _street = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _houseNumber = string.Empty;
    public required string HouseNumber
    {
        get => _houseNumber;
        set => _houseNumber = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _city = string.Empty;
    public required string City
    {
        get => _city;
        set => _city = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _postalCode = string.Empty;
    public required string PostalCode
    {
        get => _postalCode;
        set => _postalCode = Guard.Against.NullOrWhiteSpace(value);
    }
    private string _contactPhone = string.Empty;

    public required string ContactPhone
    {
        get => _contactPhone;
        set => _contactPhone = Guard.Against.NullOrWhiteSpace(value);
    }
    private List<string> _facilities;
    public required List<string> Facilities
    {
        get => _facilities;
        set => _facilities = value;
    }

    private string _description = string.Empty;
    public required string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
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

    private List<Building>? _buildings;
    public List<Building>? Buildings
    {
        get => _buildings;
        set => _buildings = value;
    }
}
