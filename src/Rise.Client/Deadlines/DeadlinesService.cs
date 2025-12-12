using Microsoft.AspNetCore.WebUtilities;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;
using Rise.Shared.Schedule;
using System.Net;
using System.Net.Http.Json;

/// <summary>
/// Client-side service for deadline operations.
/// </summary>
/// <param name="httpClient">The HTTP client for API communication.</param>
public class DeadlinesService(HttpClient httpClient) : IDeadlineService
{
    /// <summary>
    /// Retrieves deadlines within the specified date range.
    /// </summary>
    /// <param name="request">The request containing date range and pagination options.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the list of deadlines, or an error if unauthorized.</returns>
    public async Task<Result<DeadlineResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["skip"] = request.Skip.ToString(),
            ["take"] = request.Take.ToString()
        };

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            parameters["searchterm"] = request.SearchTerm!;

        if (request.StartDate.HasValue)
            parameters["startdate"] = request.StartDate.Value.ToString("O");
        if (request.EndDate.HasValue)
            parameters["enddate"] = request.EndDate.Value.ToString("O");

        var url = QueryHelpers.AddQueryString("/api/deadlines", parameters);

        var response = await httpClient.GetAsync(url, cancellationToken: ctx);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result<DeadlineResponse.Index>.Error("Not Authorized");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Result<DeadlineResponse.Index>>(cancellationToken: ctx);
        return result!;
    }

}

