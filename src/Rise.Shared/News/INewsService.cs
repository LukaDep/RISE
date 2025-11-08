using Rise.Shared.Common;

namespace Rise.Shared.News;

public interface INewsService
{
    Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default);
    Task<Result<NewsResponse.Get>> GetByIdAsync(int id, CancellationToken ctx = default);

}