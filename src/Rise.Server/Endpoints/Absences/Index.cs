using Rise.Shared.Absences;
using Rise.Shared.Common;
using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Absences;

/// <summary>
/// List all absences.
/// </summary>
/// <param name="absencesService"></param>
public class Index(IAbsencesService absencesService) : Endpoint<QueryRequest.SkipTake, Result<AbsencesResponse.Index>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/absences");
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Retrieves a paginated list of all absences for the current user.
    /// </summary>
    /// <param name="req">The pagination request containing skip and take values.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the list of absences.</returns>
    public override Task<Result<AbsencesResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return absencesService.GetIndexAsync(req, ct);
    }
}