using Rise.Shared.Common;

namespace Rise.Shared.Menu;

public interface IMenuService
{
    Task<Result<MenuResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
}
