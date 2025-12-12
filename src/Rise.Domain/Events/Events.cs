namespace Rise.Domain.Events;

/// <summary>
/// Represents an event or activity.
/// Contains event details including title, time period, location, type, and registration information.
/// </summary>
public class Event : Entity
{
    /// <summary>
    /// The title of the event.
    /// </summary>
    private string _title = string.Empty;
    public required string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The start date and time of the event. Stored in UTC.
    /// </summary>
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

    /// <summary>
    /// The end date and time of the event. Stored in UTC.
    /// </summary>
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

    /// <summary>
    /// The location where the event takes place.
    /// </summary>
    private string _location = string.Empty;
    public required string Location
    {
        get => _location;
        set => _location = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// Optional URL for event registration.
    /// </summary>
    private string? _registrationLink;
    public string? RegistrationLink
    {
        get => _registrationLink;
        set => _registrationLink = value;
    }

    /// <summary>
    /// The type or category of the event.
    /// </summary>
    private string _type = string.Empty;
    public required string Type
    {
        get => _type;
        set => _type = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// Optional description of the event.
    /// </summary>
    private string? _description;
    public string? Description
    {
        get => _description;
        set => _description = value;
    }
}
