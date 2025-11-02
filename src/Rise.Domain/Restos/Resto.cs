namespace Rise.Domain.Restos;

public class Resto : Entity
{
  private string _name = string.Empty;
  public required string Name
  {
    get => _name;
    set => _name = Guard.Against.NullOrWhiteSpace(value);
  }

  private string _description = string.Empty;
  public string? Description
  {
    get => _description;
    set => _description = value != null ? Guard.Against.NullOrWhiteSpace(value) : string.Empty;
  }

  private string _buildingId = string.Empty;
  public required string BuildingId
  {
    get => _buildingId;
    set => _buildingId = Guard.Against.NullOrWhiteSpace(value);
  }

  private Dictionary<DayOfWeek, string>? _openingHours;
  public Dictionary<DayOfWeek, string>? OpeningHours
  {
    get => _openingHours;
    set => _openingHours = value;
  }

  private bool _isCurrentlyOpen;
  public bool IsCurrentlyOpen
  {
    get => _isCurrentlyOpen;
    set => _isCurrentlyOpen = value;
  }

  private List<string>? _kitchenType;
  public List<string>? KitchenType
  {
    get => _kitchenType;
    set => _kitchenType = value;
  }

  private string? _phoneNumber;
  public string? PhoneNumber
  {
    get => _phoneNumber;
    set => _phoneNumber = value;
  }

  private string? _email;
  public string? Email
  {
    get => _email;
    set => _email = value;
  }

  private string? _imageUrl;
  public string? ImageUrl
  {
    get => _imageUrl;
    set => _imageUrl = value;
  }
}
