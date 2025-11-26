namespace Rise.Domain.Events;

public class Event : Entity
{
    private string _title = string.Empty;
    public required string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }

    private DateTime _startDateTime;
    public DateTime StartDateTime
    {
        get => _startDateTime;
        set
        {
            Guard.Against.Default(value, nameof(StartDateTime));
            _startDateTime = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();

        }
    }

    private DateTime _endDateTime;
    public DateTime EndDateTime
    {
        get => _endDateTime;
        set
        {
            Guard.Against.Default(value, nameof(EndDateTime));
            _endDateTime = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }
    }

    private string _location = string.Empty;
    public required string Location
    {
        get => _location;
        set => _location = Guard.Against.NullOrWhiteSpace(value);
    }

    private string? _registrationLink;
    public string? RegistrationLink
    {
        get => _registrationLink;
        set => _registrationLink = value;
    }

    private string _type = string.Empty;
    public required string Type
    {
        get => _type;
        set => _type = Guard.Against.NullOrWhiteSpace(value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => _description = value;
    }
}
