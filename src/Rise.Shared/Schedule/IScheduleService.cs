using Rise.Shared.Common;

namespace Rise.Shared.Schedule;

/// <summary>
/// Service interface for managing class schedules.
/// </summary>
public interface IScheduleService
{
    /// <summary>
    /// Retrieves schedule data filtered by date range.
    /// Marks classes where the teacher is absent.
    /// </summary>
    /// <param name="req">QueryRequest.DateRange with StartDate and EndDate for filtering</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with ScheduleDto.Data containing the list of schedule items, or NotFound/Error on problems</returns>
    Task<Result<ScheduleDto.Data>> GetIndexAsync(QueryRequest.DateRange req, CancellationToken ctx = default);
}