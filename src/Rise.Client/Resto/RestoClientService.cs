using Rise.Shared.Common;
using Rise.Shared.Resto;
using System.Net.Http.Json;

namespace Rise.Client.Resto;

/// <summary>
/// Client-side service for restaurant operations.
/// </summary>
/// <param name="httpClient">The HTTP client for API communication.</param>
public class RestoClientService(HttpClient httpClient) : IRestoService
{
    private readonly HttpClient httpClient = httpClient;

    /// <summary>
    /// Retrieves a paginated list of all restaurants.
    /// </summary>
    /// <param name="req">The pagination request.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the list of restaurants.</returns>
    public async Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<RestoResponse.Index>>($"/api/restos?skip={req.Skip}&take={req.Take}&searchTerm={req.SearchTerm ?? ""}", cancellationToken: ctx);
        return result!;
    }
}
