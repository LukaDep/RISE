using Rise.Shared.Common;
using Rise.Shared.Campus;

namespace Rise.Server.Endpoints.Campus;

/// <summary>
/// Get a campus plan by ID.
/// </summary>
/// <param name="campusService">The campus service.</param>
public class GetCampusById(ICampusService campusService) : EndpointWithoutRequest<Result<CampusResponse.Get>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/campuses/{id}");
        AllowAnonymous();
    }

    /// <summary>
    /// Retrieves a specific campus by its unique identifier.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the campus details.</returns>
    public override async Task<Result<CampusResponse.Get>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        return await campusService.GetCampusByIdAsync(id, ct);
    }
}