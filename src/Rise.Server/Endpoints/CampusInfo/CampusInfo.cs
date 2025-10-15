using Rise.Shared.Common;
using Rise.Shared.CampusInfo;
using Rise.Client.CampusInfo;

namespace Rise.Server.Endpoints.CampusInfo;

/// <summary>
/// List all campussen.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="campusInfoService"></param>
public class CampusInfo(ICampusInfoService campusInfoService) : Endpoint<QueryRequest.SkipTake, Result<CampusInfoResponse.Index>>
{
    public override void Configure()
    {
        Get("/api/campusInfo");
        AllowAnonymous();
    }

    public override Task<Result<CampusInfoResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return campusInfoService.GetIndexAsync(req, ct);
    }
}
