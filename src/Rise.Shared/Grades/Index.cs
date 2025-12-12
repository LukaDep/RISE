namespace Rise.Shared.Grades;

/// <summary>
/// Response wrappers for grade-related operations.
/// </summary>
public static partial class GradesResponse
{
    /// <summary>
    /// Response containing a paginated list of grades.
    /// Used for listing student grades across courses.
    /// </summary>
    public class Index
    {
        public IEnumerable<GradesDto.Grade> Grades { get; set; } = [];
    }

    /// <summary>
    /// Response containing a single grade record.
    /// Used for detail views of individual grades.
    /// </summary>
    public class Get
    {
        public GradesDto.Grade Grade { get; set; } = default!;
    }
}
