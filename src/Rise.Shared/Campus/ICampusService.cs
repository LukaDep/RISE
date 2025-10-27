namespace Rise.Shared.Campus;

using Rise.Shared.Common;

// <summary>
// Provides methods for managing campus-related operations.
// </summary>
public interface ICampusService
{
    Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
    Task<Result<CampusDto.Index>> GetCampusByIdAsync(string id, CancellationToken ct = default);
    Task<Result<BuildingDto.Index>> GetBuildingByIdAsync(string id, CancellationToken ct = default);
}