namespace Rise.Domain.Absences;

public class Absence : Entity
{
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

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

    private string _reason = string.Empty;
    public required string Reason
    {
        get => _reason;
        set => _reason = Guard.Against.NullOrWhiteSpace(value);
    }
}
