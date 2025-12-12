namespace Rise.Domain.Restos;

/// <summary>
/// Represents a restaurant or cafeteria on campus.
/// Contains details including name, description, location, opening hours, kitchen types, and contact information.
/// </summary>
public class Resto : Entity
{
    /// <summary>
    /// The name of the restaurant.
    /// </summary>
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// A description of the restaurant.
    /// </summary>
    private string _description = string.Empty;
    public string? Description
    {
        get => _description;
        set => _description = value != null ? Guard.Against.NullOrWhiteSpace(value) : string.Empty;
    }

    /// <summary>
    /// The unique identifier of the building where the restaurant is located.
    /// </summary>
    private Guid _buildingId = Guid.Empty;
    public required Guid BuildingId
    {
        get => _buildingId;
        set => _buildingId = Guard.Against.Default(value);
    }

    /// <summary>
    /// Opening hours per day of the week. Format: "HH:mm-HH:mm" for each day.
    /// </summary>
    private Dictionary<DayOfWeek, string>? _openingHours;
    public Dictionary<DayOfWeek, string>? OpeningHours
    {
        get => _openingHours;
        set => _openingHours = value;
    }

    /// <summary>
    /// Indicates whether the restaurant is currently open.
    /// </summary>
    private bool _isCurrentlyOpen;
    public bool IsCurrentlyOpen
    {
        get => _isCurrentlyOpen;
        set => _isCurrentlyOpen = value;
    }

    /// <summary>
    /// List of kitchen types or cuisine categories offered.
    /// </summary>
    private List<string>? _kitchenType;
    public List<string>? KitchenType
    {
        get => _kitchenType;
        set => _kitchenType = value;
    }

    /// <summary>
    /// The contact phone number for the restaurant.
    /// </summary>
    private string? _phoneNumber;
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = value;
    }

    /// <summary>
    /// The contact email address for the restaurant.
    /// </summary>
    private string? _email;
    public string? Email
    {
        get => _email;
        set => _email = value;
    }

    /// <summary>
    /// URL to the restaurant's image.
    /// </summary>
    private string? _imageUrl;
    public string? ImageUrl
    {
        get => _imageUrl;
        set => _imageUrl = value;
    }
}
