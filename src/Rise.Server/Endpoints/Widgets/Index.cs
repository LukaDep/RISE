using Rise.Shared.Identity;
using Rise.Shared.Widgets;

namespace Rise.Server.Endpoints.Widgets;

/// <summary>
/// Get all widgets for the current user.
/// </summary>
/// <param name="widgetService">The widget service.</param>
public class Index(IWidgetService widgetService) : EndpointWithoutRequest<Result<WidgetResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/widgets");
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Retrieves all widgets configured for the current user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the user's widget configuration.</returns>
    public override async Task<Result<WidgetResponse.Index>> ExecuteAsync(CancellationToken ct)
    {
        return await widgetService.GetIndexByUserIdAsync(ct);
    }
}