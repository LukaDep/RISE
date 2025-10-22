using Rise.Shared.Common;

namespace Rise.Shared.Menus;

public interface IMenuService
{
    /// <summary>
    /// Haalt een lijst van menu's op (eventueel met paginatie via Skip/Take).
    /// </summary>
    /// <param name="request">Paginatie of filter parameters</param>
    /// <param name="ctx">Cancellation token</param>
    /// <returns>Een Result met de MenuResponse.Index</returns>
    Task<Result<MenuResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
}
