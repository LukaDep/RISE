using Rise.Shared.News;
using System.Net.Http.Json;
using Rise.Shared.Common;

namespace Rise.Client.News;

public class NewsService(HttpClient httpClient): INewsService
{
    public async Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<NewsResponse.Index>>($"${{/api/news}}?searchterm={request.SearchTerm}", cancellationToken: ctx);
        return result!;
    }
}