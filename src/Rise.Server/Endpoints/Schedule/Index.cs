using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Server.Endpoints.Schedule;

/// <summary>
/// List all products.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="ScheduleService"></param>
public class Index(IScheduleService ScheduleService) : Endpoint<QueryRequest.SkipTake, Result<ScheduleDto.Data>>
{
    public override void Configure()
    {
        Get("/api/schedules");
        AllowAnonymous();
    }

    public override Task<Result<ScheduleDto.Data>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        return ScheduleService.GetIndexAsync(req, ct);
    }
}