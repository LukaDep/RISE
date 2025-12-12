namespace Rise.Domain.Campus;

/// <summary>
/// Represents a campus location with address, facilities, contact information, and associated buildings.
/// </summary>
public class Campus : Entity
{
    /// <summary>
    /// The name of the campus.
    /// </summary>
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The street name of the campus address.
    /// </summary>
    private string _street = string.Empty;
    public required string Street
    {
        get => _street;
        set => _street = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The house number of the campus address.
    /// </summary>
    private string _houseNumber = string.Empty;
    public required string HouseNumber
    {
        get => _houseNumber;
        set => _houseNumber = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The city where the campus is located.
    /// </summary>
    private string _city = string.Empty;
    public required string City
    {
        get => _city;
        set => _city = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The postal code of the campus address.
    /// </summary>
    private string _postalCode = string.Empty;
    public required string PostalCode
    {
        get => _postalCode;
        set => _postalCode = Guard.Against.NullOrWhiteSpace(value);
    }
    /// <summary>
    /// List of facilities available at the campus (e.g., library, cafeteria, gym).
    /// </summary>
    private List<string> _facilities;
    public required List<string> Facilities
    {
        get => _facilities;
        set => _facilities = value;
    }

    /// <summary>
    /// A description of the campus.
    /// </summary>
    private string _description = string.Empty;
    public required string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The latitude coordinate of the campus location.
    /// </summary>
    private double? _latitude;
    public double? Latitude
    {
        get => _latitude;
        set => _latitude = value;
    }

    /// <summary>
    /// The longitude coordinate of the campus location.
    /// </summary>
    private double? _longitude;
    public double? Longitude
    {
        get => _longitude;
        set => _longitude = value;
    }

    /// <summary>
    /// Collection of buildings associated with this campus.
    /// </summary>
    private List<Building>? _buildings;
    public List<Building>? Buildings
    {
        get => _buildings;
        set => _buildings = value;
    }
    /// <summary>
    /// The contact phone number for the campus.
    /// </summary>
    private string _contactPhone = string.Empty;
    public required string ContactPhone
    {
        get => _contactPhone;
        set => _contactPhone = Guard.Against.NullOrWhiteSpace(value);
    }

}
