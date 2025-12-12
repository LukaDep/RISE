namespace Rise.Domain.Deadlines;

/// <summary>
/// Represents an assignment or task deadline for a student.
/// Contains deadline details including due date, instructor, title, description, and associated course.
/// </summary>
public class Deadline : Entity
{
    /// <summary>
    /// The associated user's unique identifier (references IdentityUser.Id).
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// The due date of the deadline. Stored in UTC.
    /// </summary>
    private DateTime _endDate;

    public DateTime EndDate
    {
        get => _endDate;
        set => _endDate = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
    }

    /// <summary>
    /// The name of the instructor who assigned the deadline.
    /// </summary>
    private string _lector;
    public string Lector
    {
        get => _lector;
        set => _lector = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The title of the assignment or task.
    /// </summary>
    private string _title;

    public string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }
    /// <summary>
    /// Optional description of the assignment or task.
    /// </summary>
    private string? _description;
    public string? Description
    {
        get => _description;
        set => _description = value;
    }

    /// <summary>
    /// The course associated with this deadline.
    /// </summary>
    private string? _course;

    public string? Course
    {
        get => _course;
        set => _course = value;
    }
}