using Rise.Shared.Common;
using Rise.Shared.Campus;

namespace Rise.Server.Endpoints.Campus;

/// <summary>
/// Get a building by building code.
/// </summary>
/// <param name="buildingService">The campus service for building operations.</param>
public class GetBuildingById(ICampusService buildingService) : EndpointWithoutRequest<Result<BuildingResponse.Get>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/buildings/{code}");
        AllowAnonymous();
    }

    /// <summary>
    /// Retrieves a specific building by its building code.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the building details.</returns>
    public override async Task<Result<BuildingResponse.Get>> ExecuteAsync(CancellationToken ct)
    {
        var code = Route<string>("code");
        return await buildingService.GetBuildingByBuildingCodeAsync(code, ct);
    }
}