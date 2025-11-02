namespace Rise.Domain.Contact;

public class Contact : Entity
{
  private string _type = string.Empty;
  public required string Type
  {
    get => _type;
    set => _type = Guard.Against.NullOrWhiteSpace(value);
  }

  private string _name = string.Empty;
  public required string Name
  {
    get => _name;
    set => _name = Guard.Against.NullOrWhiteSpace(value);
  }

  private string? _email;
  public string? Email
  {
    get => _email;
    set => _email = value;
  }

  private string? _phoneNumber;
  public string? PhoneNumber
  {
    get => _phoneNumber;
    set => _phoneNumber = value;
  }

  private string? _contactPerson;
  public string? ContactPerson
  {
    get => _contactPerson;
    set => _contactPerson = value;
  }

  private List<string>? _campusses;
  public List<string>? Campusses
  {
    get => _campusses;
    set => _campusses = value;
  }
}
