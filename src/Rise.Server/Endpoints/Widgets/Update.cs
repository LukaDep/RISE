using Rise.Shared.Identity;
using Rise.Shared.Widgets;

namespace Rise.Server.Endpoints.Widgets;

/// <summary>
/// Update the widgets for the current user.
/// </summary>
/// <param name="widgetService">The widget service.</param>
public class Update(IWidgetService widgetService) : Endpoint<WidgetRequest.Update, Result>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Put("/api/widgets");
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Updates the widget configuration for the current user.
    /// </summary>
    /// <param name="req">The update request containing the new widget configuration.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public override Task<Result> ExecuteAsync(WidgetRequest.Update req, CancellationToken ctx)
    {
        return widgetService.UpdateUserWidgetsAsync(req, ctx);
    }
}