using Rise.Shared.Common;

namespace Rise.Shared.Resto;

/// <summary>
/// Service interface for managing restaurant-related operations.
/// </summary>
public interface IRestoService
{
    /// <summary>
    /// Retrieves a filtered and paginated list of restaurants.
    /// Supports searching by name, description and kitchen type, and determines if a restaurant is currently open.
    /// </summary>
    /// <param name="req">QueryRequest.SkipTake with SearchTerm, Skip and Take for filtering and pagination</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with RestoResponse.Index containing the list of restaurants</returns>
    Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default);
}