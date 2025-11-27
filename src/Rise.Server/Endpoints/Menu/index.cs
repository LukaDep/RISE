using Rise.Shared.Menu;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.Menu;

/// <param name="menuService"></param>
public class Index(IMenuService menuService)
    : Endpoint<QueryRequest.DateRange, Result<MenuResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/menus");
        AllowAnonymous();
    }

    public override Task<Result<MenuResponse.Index>> ExecuteAsync(QueryRequest.DateRange req, CancellationToken ct)
    {
        return menuService.GetIndexAsync(req, ct);
    }
}
