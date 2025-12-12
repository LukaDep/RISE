using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

/// <summary>
/// Client-side service for news article operations.
/// </summary>
/// <param name="httpClient">The HTTP client for API communication.</param>
public class NewsService(HttpClient httpClient) : INewsService
{
    /// <summary>
    /// Retrieves news articles within the specified date range.
    /// </summary>
    /// <param name="request">The request containing date range and pagination options.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the list of news articles.</returns>
    public async Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
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

        var url = QueryHelpers.AddQueryString("/api/news", parameters);

        var result = await httpClient.GetFromJsonAsync<Result<NewsResponse.Index>>(url, cancellationToken: ctx);
        return result!;
    }

    /// <summary>
    /// Retrieves a specific news article by its unique identifier.
    /// </summary>
    /// <param name="id">The news article ID.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the news article details.</returns>
    public async Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        var response = await httpClient.GetAsync($"/api/news/{id}", cancellationToken: ctx);
        var result = await response.Content.ReadFromJsonAsync<Result<NewsResponse.Get>>(cancellationToken: ctx);
        return result!;
    }
}