using Rise.Shared.Common;

namespace Rise.Shared.Resto;

/// <summary>
/// Provides methods for managing Resto-related operations.
/// </summary>
public interface IRestoService
{
    Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default);
}