namespace Rise.Domain.CampusInfo;

/// <summary>
/// Represents campus information with location, faculties, and contact details.
/// This is a simplified campus representation focusing on informational aspects.
/// </summary>
public class CampusInfo : Entity
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
    /// The location or address of the campus.
    /// </summary>
    private string _location = string.Empty;
    public required string Location
    {
        get => _location;
        set => _location = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// List of faculties or departments at the campus.
    /// </summary>
    private List<string> _faculties = new();
    public required List<string> Faculties
    {
        get => _faculties;
        set => _faculties = value;
    }

    /// <summary>
    /// The contact phone number for the campus.
    /// </summary>
    private string? _contactPhone;
    public string? ContactPhone
    {
        get => _contactPhone;
        set => _contactPhone = value;
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
}
