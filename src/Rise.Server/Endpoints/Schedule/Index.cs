using Rise.Shared.Schedule;
using Rise.Shared.Common;
using Rise.Shared.Identity;

namespace Rise.Server.Endpoints.Schedule;

/// <summary>
/// List all schedules within a date range.
/// </summary>
/// <param name="ScheduleService">The schedule service.</param>
public class Index(IScheduleService ScheduleService) : Endpoint<QueryRequest.DateRange, Result<ScheduleDto.Data>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/schedules");
        Roles(AppRoles.Student);
    }

    /// <summary>
    /// Retrieves all schedules within the specified date range for the current user.
    /// </summary>
    /// <param name="req">The date range request containing start and end dates.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the schedule data.</returns>
    public override Task<Result<ScheduleDto.Data>> ExecuteAsync(QueryRequest.DateRange req, CancellationToken ct)
    {
        return ScheduleService.GetIndexAsync(req, ct);
    }
}