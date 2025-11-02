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

  private string? _mapImageUrl;
  public string? MapImageUrl
  {
    get => _mapImageUrl;
    set => _mapImageUrl = value;
  }

  private int? _imageWidth;
  public int? ImageWidth
  {
    get => _imageWidth;
    set => _imageWidth = value;
  }

  private int? _imageHeight;
  public int? ImageHeight
  {
    get => _imageHeight;
    set => _imageHeight = value;
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
