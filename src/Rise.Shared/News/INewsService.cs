using Rise.Shared.Common;

namespace Rise.Shared.News;

public interface INewsService
{
    Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
    Task<Result<NewsResponse.Get>> GetByIdAsync(string id, CancellationToken ctx = default);

}