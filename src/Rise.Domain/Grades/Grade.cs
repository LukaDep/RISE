namespace Rise.Domain.Grades;

public class Grade : Entity
{
  private string _name = string.Empty;
  public required string Name
  {
    get => _name;
    set => _name = Guard.Against.NullOrWhiteSpace(value);
  }

  private string _activityType = string.Empty;
  public required string ActivityType
  {
    get => _activityType;
    set => _activityType = Guard.Against.NullOrWhiteSpace(value);
  }

  private double? _maxPoints;
  public double? MaxPoints
  {
    get => _maxPoints;
    set => _maxPoints = value;
  }

  private double? _score;
  public double? Score
  {
    get => _score;
    set => _score = value;
  }

  private string? _feedback;
  public string? Feedback
  {
    get => _feedback;
    set => _feedback = value;
  }

  private DateTime? _submissionDate;
  public DateTime? SubmissionDate
  {
    get => _submissionDate;
    set => _submissionDate = value.HasValue
        ? (value.Value.Kind == DateTimeKind.Utc ? value.Value : value.Value.ToUniversalTime())
        : null;
  }

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

  private string? _courseId;
  public string? CourseId
  {
    get => _courseId;
    set => _courseId = value;
  }

  private string? _courseName;
  public string? CourseName
  {
    get => _courseName;
    set => _courseName = value;
  }

  private string? _year;
  public string? Year
  {
    get => _year;
    set => _year = value;
  }

  private int? _semester;
  public int? Semester
  {
    get => _semester;
    set => _semester = value;
  }
}
