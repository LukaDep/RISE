using Rise.Shared.Absences;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.Absences;

/// <summary>
/// List all absences.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="absencesService"></param>
public class Index(IAbsencesService absencesService) : Endpoint<QueryRequest.SkipTake, Result<AbsencesResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/absences");
        AllowAnonymous();
    }

    public override Task<Result<AbsencesResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return absencesService.GetIndexAsync(req, ct);
    }
}