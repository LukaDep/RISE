using Rise.Shared.Common;
using Rise.Shared.Campus;

namespace Rise.Server.Endpoints.Campus;

// <summary>
// Get a campus plan by ID.
// </summary>
public class GetById(ICampusService campusService) : EndpointWithoutRequest<Result<CampusDto.Index>>
{
    public override void Configure()
    {
        Get("/api/campuses/{id}");
        AllowAnonymous();
    }

    public override async Task<Result<CampusDto.Index>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<string>("id");
        return await campusService.GetByIdAsync(id, ct);
    }
}