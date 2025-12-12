using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;

using Rise.Shared.Absences;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Services.Absences;

/// <summary>
/// Service for managing teacher absences.
/// </summary>
public class AbsencesService(ApplicationDbContext dbContext) : IAbsencesService
{
    /// <summary>
    /// Retrieves a paginated and sorted list of absences.
    /// Defaults to sorting by start date.
    /// </summary>
    /// <param name="request">QueryRequest.SkipTake with OrderBy, Skip and Take</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with AbsencesResponse.Index containing the list of absences</returns>
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