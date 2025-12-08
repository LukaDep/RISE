using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;

namespace Rise.Services.Deadlines;

public class DeadlineService(ApplicationDbContext dbContext) : IDeadlineService
{
    public async Task<Result<DeadlineResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var query = dbContext.Deadlines.AsQueryable();
        DateTime? start = request.StartDate;
        DateTime? end = request.EndDate;

        if (start.HasValue && start.Value == default(DateTime))
            start = null;
        if (end.HasValue && end.Value == default(DateTime))
            end = null;

        if (start.HasValue && end.HasValue)
        {
            var s = start.Value.Date;
            var e = end.Value.Date;
            if (s > e)
                (s, e) = (e, s);

            query = query.Where(n => n.EndDate.Date >= s && n.EndDate.Date <= e);
        }
        else if (start.HasValue)
        {
            var s = start.Value.Date;
            query = query.Where(n => n.EndDate.Date >= s);
        }
        else if (end.HasValue)
        {
            var e = end.Value.Date;
            query = query.Where(n => n.EndDate.Date <= e);
        }
        // Apply ordering
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }
        else
        {
            // Default order
            query = query.OrderBy(p => p.EndDate);
        }
        
        var deadlines = await query.AsNoTracking()
            .Select(d => new DeadlineDto.Index
            {
                Id = d.Id,
                Title = d.Title,
                Lector = d.Lector,
                EndDate = d.EndDate,
                Description = d.Description,
                Course = d.Course
            }).ToListAsync(ctx);

        return Result.Success(new DeadlineResponse.Index
        {
            Deadlines = deadlines,
        });
    }
}