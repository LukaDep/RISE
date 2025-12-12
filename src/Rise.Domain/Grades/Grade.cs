namespace Rise.Domain.Grades;

/// <summary>
/// Represents a student grade record for an assignment, exam, or activity.
/// Contains grade details, feedback, and associated course information.
/// </summary>
public class Grade : Entity
{
    /// <summary>
    /// The name of the graded assignment or activity.
    /// </summary>
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The type of activity (e.g., exam, assignment, project).
    /// </summary>
    private string _activityType = string.Empty;
    public required string ActivityType
    {
        get => _activityType;
        set => _activityType = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The maximum points achievable for this grade.
    /// </summary>
    private double? _maxPoints;
    public double? MaxPoints
    {
        get => _maxPoints;
        set => _maxPoints = value;
    }

    /// <summary>
    /// The score achieved by the student.
    /// </summary>
    private double? _score;
    public double? Score
    {
        get => _score;
        set => _score = value;
    }

    /// <summary>
    /// Optional feedback provided by the instructor.
    /// </summary>
    private string? _feedback;
    public string? Feedback
    {
        get => _feedback;
        set => _feedback = value;
    }

    /// <summary>
    /// The date when the assignment was submitted. Stored in UTC.
    /// </summary>
    private DateTime? _submissionDate;
    public DateTime? SubmissionDate
    {
        get => _submissionDate;
        set => _submissionDate = value.HasValue
            ? (value.Value.Kind == DateTimeKind.Utc ? value.Value : value.Value.ToUniversalTime())
            : null;
    }

    /// <summary>
    /// The date of the grade record. Stored in UTC.
    /// </summary>
    private DateTime _date;
    public DateTime Date
    {
        get => _date;
        set
        {
            Guard.Against.Default(value, nameof(Date));
            _date = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }
    }

    /// <summary>
    /// The identifier of the course this grade belongs to.
    /// </summary>
    private string? _courseId;
    public string? CourseId
    {
        get => _courseId;
        set => _courseId = value;
    }

    /// <summary>
    /// The name of the course.
    /// </summary>
    private string? _courseName;
    public string? CourseName
    {
        get => _courseName;
        set => _courseName = value;
    }

    /// <summary>
    /// The academic year of the course.
    /// </summary>
    private string? _year;
    public string? Year
    {
        get => _year;
        set => _year = value;
    }

    /// <summary>
    /// The semester number of the course.
    /// </summary>
    private int? _semester;
    public int? Semester
    {
        get => _semester;
        set => _semester = value;
    }

    /// <summary>
    /// The unique identifier of the student who owns this grade.
    /// </summary>
    public string? UserId { get; set; }
}
