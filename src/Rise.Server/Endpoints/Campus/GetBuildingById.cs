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
        Get("/api/buildings/{id}");
        AllowAnonymous();
    }

    public override async Task<Result<BuildingResponse.Get>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        return await buildingService.GetBuildingByIdAsync(id, ct);
    }
}