namespace Rise.Shared.Schedule;

using Rise.Shared.Common;

/// <summary>
/// Contains data transfer objects (DTOs) used for schedule data retrieval.
/// </summary>
public static class ScheduleDto
{
    /// <summary>
    /// Applies date range filtering to a collection of schedules based on the provided request.
    /// </summary>
    /// <param name="schedules">The collection of schedules to filter.</param>
    /// <param name="req">The date range request containing start and end dates.</param>
    /// <returns>The filtered collection of schedules.</returns>
    public static IEnumerable<Schedule> ApplyDateRangeFilter(IEnumerable<Schedule> schedules, QueryRequest.DateRange req)
    {
        var query = schedules.AsQueryable();

        if (req.StartDate.HasValue || req.EndDate.HasValue)
        {
            if (req.StartDate.HasValue && req.EndDate.HasValue)
            {
                var start = req.StartDate.Value.Date;
                var end = req.EndDate.Value.Date;
                if (start > end)
                    (start, end) = (end, start);

                query = query.Where(s => s.StartDateTime.Date >= start && s.StartDateTime.Date <= end);
            }
            else if (req.StartDate.HasValue)
            {
                var start = req.StartDate.Value.Date;
                query = query.Where(s => s.StartDateTime.Date >= start);
            }
            else if (req.EndDate.HasValue)
            {
                var end = req.EndDate.Value.Date;
                query = query.Where(s => s.StartDateTime.Date <= end);
            }
        }

        return query;
    }

    /// <summary>
    /// Represents the API response containing a list of schedules.
    /// </summary>
    public class Data
    {
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }

    // Schedule object
    public class Schedule
    {
        public string Id { get; set; } = default!;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Course { get; set; } = default!;
        public string WorkForm { get; set; } = default!;
        public string Environment { get; set; } = default!;
        public string Room { get; set; } = default!;
        public string Teacher { get; set; } = default!;
        public bool IsAbsent { get; set; } = false;
    }
}