using Rise.Shared.Common;
using Rise.Shared.Campus;

namespace Rise.Server.Endpoints.Campus;

// <summary>
// Get a building by ID.
// </summary>
public class GetBuildingById(ICampusService buildingService) : EndpointWithoutRequest<Result<BuildingResponse.Get>>
{
    public override void Configure()
    {
        Get("/api/buildings/{code}");
        AllowAnonymous();
    }

    public override async Task<Result<BuildingResponse.Get>> ExecuteAsync(CancellationToken ct)
    {
        var code = Route<string>("code");
        return await buildingService.GetBuildingByBuildingCodeAsync(code, ct);
    }
}