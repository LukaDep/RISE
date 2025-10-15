using System.Net.Http.Json;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public class NewsService(HttpClient httpClient) : INewsService
{
    public async Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<NewsResponse.Index>>($"/api/news", cancellationToken: ctx);
        return result!;
    }

    public async Task<Result<NewsResponse.Get>> GetByIdAsync(int id, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<NewsResponse.Get>>($"/api/news/{id}", cancellationToken: ctx);
        return result!;
    }
}