using Rise.Shared.Common;
using Rise.Shared.Campus;

namespace Rise.Server.Endpoints.Campus;

// <summary>
// Get a campus plan by ID.
// </summary>
public class GetCampusById(ICampusService campusService) : EndpointWithoutRequest<Result<CampusResponse.Get>>
{
    public override void Configure()
    {
        Get("/api/campuses/{id}");
        AllowAnonymous();
    }

    public override async Task<Result<CampusResponse.Get>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        return await campusService.GetCampusByIdAsync(id, ct);
    }
}