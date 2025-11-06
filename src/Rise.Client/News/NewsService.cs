using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public class NewsService(HttpClient httpClient) : INewsService
{
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

    public async Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        var response = await httpClient.GetAsync($"/api/news/{id}", cancellationToken: ctx);
        var result = await response.Content.ReadFromJsonAsync<Result<NewsResponse.Get>>(cancellationToken: ctx);
        return result!;
    }
}