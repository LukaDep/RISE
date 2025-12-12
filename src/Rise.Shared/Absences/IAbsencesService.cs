using Rise.Shared.Common;

namespace Rise.Shared.Absences;

/// <summary>
/// Service interface for managing teacher absences.
/// </summary>
public interface IAbsencesService
{
    /// <summary>
    /// Retrieves a paginated and sorted list of absences.
    /// Defaults to sorting by start date.
    /// </summary>
    /// <param name="request">QueryRequest.SkipTake with OrderBy, Skip and Take</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with AbsencesResponse.Index containing the list of absences</returns>
    Task<Result<AbsencesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
}