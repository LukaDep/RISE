using Rise.Shared.Campus;
using System.Net.Http.Json;
using Rise.Shared.Common;

namespace Rise.Client.Campus;

/// <summary>
/// Client-side service for campus and building operations.
/// </summary>
/// <param name="httpClient">The HTTP client for API communication.</param>
public class CampusClientService(HttpClient httpClient) : ICampusService
{
    /// <summary>
    /// Retrieves a paginated list of all campuses.
    /// </summary>
    /// <param name="request">The pagination request.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the list of campuses.</returns>
    public async Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<CampusResponse.Index>>($"/api/campuses", cancellationToken: ctx);
        Console.WriteLine(@"Fetching campus data from API...");
        Console.WriteLine(result);
        return result?.Value!;
    }

    /// <summary>
    /// Retrieves a specific campus by its unique identifier.
    /// </summary>
    /// <param name="buildingId">The campus ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the campus details.</returns>
    public async Task<Result<CampusResponse.Get>> GetCampusByIdAsync(Guid buildingId, CancellationToken ct = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<CampusResponse.Get>>($"/api/campuses/{buildingId}", cancellationToken: ct);
        return result!;
    }

    /// <summary>
    /// Retrieves a specific building by its building code.
    /// </summary>
    /// <param name="code">The building code.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the building details.</returns>
    public async Task<Result<BuildingResponse.Get>> GetBuildingByBuildingCodeAsync(string code, CancellationToken ct = default)
    {

        var result = await httpClient.GetFromJsonAsync<Result<BuildingResponse.Get>>($"/api/buildings/{code}", cancellationToken: ct);
        Console.WriteLine($@"Fetching building data for ID {code} from API...");
        return result!;
    }

}