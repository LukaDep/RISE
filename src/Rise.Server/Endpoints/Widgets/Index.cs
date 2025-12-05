using Rise.Shared.Identity;
using Rise.Shared.Widgets;

namespace Rise.Server.Endpoints.Widgets;

public class Index(IWidgetService widgetService) : EndpointWithoutRequest<Result<WidgetResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/widgets");
        Roles(AppRoles.Student);
    }

    public override async Task<Result<WidgetResponse.Index>> ExecuteAsync(CancellationToken ct)
    {
        return await widgetService.GetIndexByUserIdAsync(ct);
    }
}