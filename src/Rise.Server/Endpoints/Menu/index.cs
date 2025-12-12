using Rise.Shared.Menu;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.Menu;

/// <summary>
/// List all menus within a date range.
/// </summary>
/// <param name="menuService">The menu service.</param>
public class Index(IMenuService menuService)
    : Endpoint<QueryRequest.DateRange, Result<MenuResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/menus");
        AllowAnonymous();
    }

    /// <summary>
    /// Retrieves all menus within the specified date range.
    /// </summary>
    /// <param name="req">The date range request containing start and end dates.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the list of menus.</returns>
    public override Task<Result<MenuResponse.Index>> ExecuteAsync(QueryRequest.DateRange req, CancellationToken ct)
    {
        return menuService.GetIndexAsync(req, ct);
    }
}
