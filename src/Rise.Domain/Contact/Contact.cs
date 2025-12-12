namespace Rise.Domain.Contact;

/// <summary>
/// Represents a contact entry for a department, service, or individual.
/// Contains contact details including type, name, email, phone, contact person, and associated campuses.
/// </summary>
public class Contact : Entity
{
    /// <summary>
    /// The type or category of the contact (e.g., department, service).
    /// </summary>
    private string _type = string.Empty;
    public required string Type
    {
        get => _type;
        set => _type = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The name of the contact or department.
    /// </summary>
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The email address for the contact.
    /// </summary>
    private string? _email;
    public string? Email
    {
        get => _email;
        set => _email = value;
    }

    /// <summary>
    /// The phone number for the contact.
    /// </summary>
    private string? _phoneNumber;
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = value;
    }

    /// <summary>
    /// The name of the primary contact person.
    /// </summary>
    private string? _contactPerson;
    public string? ContactPerson
    {
        get => _contactPerson;
        set => _contactPerson = value;
    }

    /// <summary>
    /// List of campus names where this contact is available.
    /// </summary>
    private IEnumerable<string>? _campusses;
    public IEnumerable<string>? Campusses
    {
        get => _campusses;
        set => _campusses = value;
    }
}
