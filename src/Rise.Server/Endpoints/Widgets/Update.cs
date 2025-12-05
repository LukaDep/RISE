using Rise.Shared.Identity;
using Rise.Shared.Widgets;

namespace Rise.Server.Endpoints.Widgets;


/// Creation of a <see cref="Widgets"/>
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="widgetService"></param>
public class Update(IWidgetService widgetService) : Endpoint<WidgetRequest.Update, Result>
{
    public override void Configure()
    {
        Put("/api/widgets");
        Roles(AppRoles.Student);
    }

    public override Task<Result> ExecuteAsync(WidgetRequest.Update req, CancellationToken ctx)
    {
        return widgetService.UpdateUserWidgetsAsync(req, ctx);
    }
}