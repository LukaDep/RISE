namespace Rise.Shared.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;


/// <summary>
/// Provides methods for managing Grades-related operations.
/// </summary>
public interface IGradesService

{
    Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default);
    Task<Result<GradesResponse.GradeById>> GetGradeByIdAsync(string id, CancellationToken ctx = default);
}