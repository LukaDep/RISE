namespace Rise.Domain.Deadlines;

public class Deadline : Entity
{
    private DateTime _endDate;

    public DateTime EndDate
    {
        get => _endDate;
        set => _endDate = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
    }

    private string _lector;
    public string Lector
    {
        get => _lector;
        set => _lector = Guard.Against.NullOrWhiteSpace(value);
    }
    
    private string _title;

    public string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }
    private string? _description;
    public string? Description
    {
        get => _description;
        set => _description = value;
    }
    
    private string? _course;

    public string? Course
    {
        get => _course;
        set => _course = value;
    }
}