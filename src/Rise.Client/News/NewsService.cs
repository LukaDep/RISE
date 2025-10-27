using System.Net.Http.Json;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public class NewsService(HttpClient httpClient) : INewsService
{
    public async Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        // Build safe request path and add better error information on failure
        var path = $"/api/news?searchterm={Uri.EscapeDataString(request.SearchTerm ?? string.Empty)}&skip={request.Skip}&take={request.Take}";
        try
        {
            var result = await httpClient.GetFromJsonAsync<Result<NewsResponse.Index>>(path, cancellationToken: ctx);
            return result!;
        }
        catch (Exception ex)
        {
            // Compose the full request URL for easier debugging
            string fullUrl;
            try
            {
                fullUrl = httpClient.BaseAddress != null ? new Uri(httpClient.BaseAddress, path).ToString() : path;
            }
            catch
            {
                fullUrl = path;
            }

            // Throw a clearer exception that includes the request URL and the original exception
            throw new HttpRequestException($"Failed fetching news from '{fullUrl}': {ex.Message}", ex);
        }
    }

    public async Task<Result<NewsResponse.Get>> GetByIdAsync(int id, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<NewsResponse.Get>>($"/api/news/{id}", cancellationToken: ctx);
        return result!;
    }
}