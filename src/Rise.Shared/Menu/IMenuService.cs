using Rise.Shared.Common;

namespace Rise.Shared.Menu;

/// <summary>
/// Service interface for managing menus and menu items.
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// Retrieves a paginated list of menus including their menu items.
    /// </summary>
    /// <param name="request">QueryRequest.SkipTake with Skip and Take for pagination</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with MenuResponse.Index containing the list of menus with their items</returns>
    Task<Result<MenuResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
}
