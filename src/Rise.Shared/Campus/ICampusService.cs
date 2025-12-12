namespace Rise.Shared.Campus;

using Rise.Shared.Common;

/// <summary>
/// Service interface for managing campus-related operations.
/// </summary>
public interface ICampusService
{
    /// <summary>
    /// Retrieves a paginated list of campuses including their buildings.
    /// </summary>
    /// <param name="request">QueryRequest.SkipTake with Skip and Take for pagination</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with CampusResponse.Index containing the list of campuses with their buildings</returns>
    Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);

    /// <summary>
    /// Retrieves a specific campus by ID including all buildings.
    /// </summary>
    /// <param name="id">The Guid of the campus to retrieve</param>
    /// <param name="ct">CancellationToken to cancel the operation</param>
    /// <returns>Result with CampusResponse.Get containing the campus, or NotFound if the campus does not exist</returns>
    Task<Result<CampusResponse.Get>> GetCampusByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a specific building by building code.
    /// </summary>
    /// <param name="code">The building code of the building to retrieve</param>
    /// <param name="ct">CancellationToken to cancel the operation</param>
    /// <returns>Result with BuildingResponse.Get containing the building, or NotFound if the building does not exist</returns>
    Task<Result<BuildingResponse.Get>> GetBuildingByBuildingCodeAsync(string code, CancellationToken ct = default);
}