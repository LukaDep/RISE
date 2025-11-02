namespace Rise.Domain.Schedule;

public class Reservation : Entity
{
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

  private string _course = string.Empty;
  public required string Course
  {
    get => _course;
    set => _course = Guard.Against.NullOrWhiteSpace(value);
  }

  private string _workForm = string.Empty;
  public required string WorkForm
  {
    get => _workForm;
    set => _workForm = Guard.Against.NullOrWhiteSpace(value);
  }

  private string _environment = string.Empty;
  public required string Environment
  {
    get => _environment;
    set => _environment = Guard.Against.NullOrWhiteSpace(value);
  }

  private string _room = string.Empty;
  public required string Room
  {
    get => _room;
    set => _room = Guard.Against.NullOrWhiteSpace(value);
  }

  private string _teacher = string.Empty;
  public required string Teacher
  {
    get => _teacher;
    set => _teacher = Guard.Against.NullOrWhiteSpace(value);
  }

  private bool _isAbsent = false;
  public bool IsAbsent
  {
    get => _isAbsent;
    set => _isAbsent = value;
  }
}
