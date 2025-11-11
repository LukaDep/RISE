namespace Rise.Shared.Campus;

using Rise.Shared.Common;

// <summary>
// Provides methods for managing campus-related operations.
// </summary>
public interface ICampusService
{
    Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
    Task<Result<CampusResponse.Get>> GetCampusByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<BuildingResponse.Get>> GetBuildingByBuildingCodeAsync(string code, CancellationToken ct = default);
}