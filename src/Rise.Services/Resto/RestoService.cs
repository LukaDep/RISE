using System;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Resto;
using Rise.Shared.Common;

namespace Rise.Services.Resto;

public class RestoService(ApplicationDbContext dbContext) : IRestoService
{

    public async Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        var query = dbContext.Restos.AsQueryable();

        // Get all restos (we'll filter client-side to support KitchenType search)
        var allRestos = await query.AsNoTracking()
            .Select(r => new RestoDto.Index
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                BuildingId = r.BuildingId,
                OpeningHours = r.OpeningHours,
                IsCurrentlyOpen = IsCurrentlyOpen(r.OpeningHours, DateTimeOffset.Now),
                KitchenType = r.KitchenType,
                PhoneNumber = r.PhoneNumber,
                Email = r.Email,
                ImageUrl = r.ImageUrl,
            }).ToListAsync(ct);

        // Apply search filter client-side (supports KitchenType JSON array search for SQLite compatibility)
        if (!string.IsNullOrWhiteSpace(req.SearchTerm))
        {
            var term = req.SearchTerm.Trim();
            allRestos = allRestos.Where(r => 
                (r.Name ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                || (r.Description ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                || (r.KitchenType != null && r.KitchenType.Any(type => type.Contains(term, StringComparison.OrdinalIgnoreCase)))
            ).ToList();
        }

        // Apply paging after filtering
        var restos = allRestos.Skip(req.Skip).Take(req.Take).ToList();

        return Result.Success(new RestoResponse.Index { Restos = restos });


    }

    private static bool IsCurrentlyOpen(Dictionary<DayOfWeek, string>? openingHours, DateTimeOffset now)
    {
        if (openingHours == null || openingHours.Count == 0)
            return false;

        var todayBase = now.Date;
        foreach (var kvp in openingHours)
        {
            var day = kvp.Key;
            var value = kvp.Value;
            if (string.IsNullOrWhiteSpace(value))
                continue;

            var ranges = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var range in ranges)
            {
                var parts = range.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length != 2)
                    continue;

                if (!TimeSpan.TryParse(parts[0], out var start))
                    continue;
                if (!TimeSpan.TryParse(parts[1], out var end))
                    continue;

                // Compute base date for the day key relative to 'now'
                var dayDiff = (int)day - (int)now.DayOfWeek;
                var baseDate = todayBase.AddDays(dayDiff);

                var startDt = baseDate.Add(start);
                DateTimeOffset endDt;
                if (end >= start)
                {
                    endDt = baseDate.Add(end);
                }
                else
                {
                    // Overnight range (e.g. 18:00-02:00) -> end is on next day
                    endDt = baseDate.AddDays(1).Add(end);
                }

                if (now >= startDt && now < endDt)
                    return true;
            }
        }

        return false;
    }
}
