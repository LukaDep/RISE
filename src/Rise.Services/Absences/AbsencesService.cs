using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;

using Rise.Shared.Absences;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Services.Absences;

public class AbsencesService(ApplicationDbContext dbContext) : IAbsencesService
{

    public async Task<Result<AbsencesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {

        var query = dbContext.Absences.AsQueryable();

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
            query = query.OrderBy(a => a.StartDate);
        }

        var absences = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(a => new AbsenceDto.Index
            {
                Id = a.Id,
                Name = a.Name,
                Reason = a.Reason,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
            }).ToListAsync(ctx);

        return Result.Success(new AbsencesResponse.Index
        {
            Absences = absences,
        }
        );
    }
}