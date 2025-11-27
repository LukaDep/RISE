using Rise.Shared.Common;

namespace Rise.Shared.Schedule;

/// <summary>
/// Provides methods for managing Schedule-related operations.
/// </summary>
public interface IScheduleService

{
    Task<Result<ScheduleDto.Data>> GetIndexAsync(QueryRequest.DateRange req, CancellationToken ctx = default);
}