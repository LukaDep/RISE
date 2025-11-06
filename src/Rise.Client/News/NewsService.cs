using System.Net.Http.Json;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public class NewsService(HttpClient httpClient) : INewsService
{
    public async Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<NewsResponse.Index>>($"/api/news?searchterm={request.SearchTerm}&skip={request.Skip}&take={request.Take}", cancellationToken: ctx);
        return result!;
    }

    public async Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        var response = await httpClient.GetAsync($"/api/news/{id}", cancellationToken: ctx);
        var result = await response.Content.ReadFromJsonAsync<Result<NewsResponse.Get>>(cancellationToken: ctx);
        return result!;
    }
}