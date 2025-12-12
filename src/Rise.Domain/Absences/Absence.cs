namespace Rise.Domain.Absences;

/// <summary>
/// Represents a teacher absence record.
/// Contains the teacher's name, absence period (start and end dates), and reason for absence.
/// </summary>
public class Absence : Entity
{
    /// <summary>
    /// The name of the absent teacher.
    /// </summary>
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The start date of the absence period. Stored in UTC.
    /// </summary>
    private DateTime _startDate;
    public required DateTime StartDate
    {
        get => _startDate;
        set
        {
            Guard.Against.Default(value, nameof(StartDate));
            _startDate = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }
    }

    /// <summary>
    /// The end date of the absence period. Stored in UTC.
    /// </summary>
    private DateTime _endDate;
    public required DateTime EndDate
    {
        get => _endDate;
        set
        {
            Guard.Against.Default(value, nameof(EndDate));
            _endDate = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }
    }

    /// <summary>
    /// The reason for the teacher's absence.
    /// </summary>
    private string _reason = string.Empty;
    public required string Reason
    {
        get => _reason;
        set => _reason = Guard.Against.NullOrWhiteSpace(value);
    }
}
