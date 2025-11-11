namespace Rise.Domain.CampusInfo;

public class CampusInfo : Entity
{
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _location = string.Empty;
    public required string Location
    {
        get => _location;
        set => _location = Guard.Against.NullOrWhiteSpace(value);
    }

    private List<string> _faculties = new();
    public required List<string> Faculties
    {
        get => _faculties;
        set => _faculties = value;
    }

    private string? _contactPhone;
    public string? ContactPhone
    {
        get => _contactPhone;
        set => _contactPhone = value;
    }

    private string _description = string.Empty;
    public required string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }
}
