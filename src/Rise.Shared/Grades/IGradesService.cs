namespace Rise.Shared.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;


/// <summary>
/// Provides methods for managing Grades-related operations.
/// </summary>
public interface IGradesService

{
    Task<Result<GradesResponse.CourseList>> GetCoursesAsync(QueryRequest.SkipTake req, CancellationToken ctx = default);
    Task<Result<GradesResponse.CourseById>> GetCourseByIdAsync(string id, CancellationToken ctx = default);
}