namespace Rise.Shared.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// Service interface for managing student grades and results.
/// </summary>
public interface IGradesService
{
    /// <summary>
    /// Retrieves a filtered and paginated list of grades for the current user.
    /// Supports searching by course name and name, and sorting. Defaults to sorting by date (newest first).
    /// Returns an empty list if the user is not logged in.
    /// </summary>
    /// <param name="req">QueryRequest.SkipTake with SearchTerm, OrderBy, Skip and Take</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with GradesResponse.Index containing the list of grades</returns>
    Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default);

    /// <summary>
    /// Retrieves a specific grade record by ID for the current user.
    /// Only the owner can view their own grades.
    /// </summary>
    /// <param name="id">The Guid of the grade record to retrieve</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with GradesResponse.Get containing the grade, Unauthorized if not logged in, or NotFound if not found</returns>
    Task<Result<GradesResponse.Get>> GetGradeByIdAsync(Guid id, CancellationToken ctx = default);
}