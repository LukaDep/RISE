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
    private readonly string _mockFilePath;

    // public MockRestoService()
    // {
    //     var currentDirectory = Directory.GetCurrentDirectory();
    //     _mockFilePath = Path.Combine(currentDirectory, "..", "Rise.Services", "Resto", "MockData", "RestoMockdata.json");
    //     Log.Information("Current directory: {CurrentDirectory}", currentDirectory);
    //     Log.Information("Looking for mock file at: {MockFilePath}", _mockFilePath);
    //     Log.Information("File exists: {FileExists}", File.Exists(_mockFilePath));
    // }

    public async Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        // if (!File.Exists(_mockFilePath))
        // {
        //     Log.Warning("Mock data file not found at: {MockFilePath}", _mockFilePath);
        //     return Result<RestoResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");
        // }
        //
        // var json = await File.ReadAllTextAsync(_mockFilePath, ct);
        // IEnumerable<RestoDto.Index> items = JsonSerializer.Deserialize<List<RestoDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        var query = dbContext.Restos.AsQueryable();


        // Basic search
        if (!string.IsNullOrWhiteSpace(req.SearchTerm))
        {
            var term = req.SearchTerm.Trim();
            query = query.Where(i => (i.Name ?? string.Empty).Contains(term)
                                  || (i.Description ?? string.Empty).Contains(term)
                                  || (i.KitchenType != null && i.KitchenType.Any(type => type.Contains(term))));
        }

        // Paging
        // var paged = items.Skip(Math.Max(0, req.Skip)).Take(req.Take <= 0 ? 20 : req.Take).ToList();
        var restos = await query.AsNoTracking()
            .Skip(req.Skip)
            .Take(req.Take)
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

        return Result.Success(new RestoResponse.Index { Restos = restos });


        // var now = DateTimeOffset.Now;
        // foreach (var resto in paged)
        // {
        //     try
        //     {
        //         resto.IsCurrentlyOpen = IsCurrentlyOpen(resto.OpeningHours, now);
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Warning(ex, "Failed to compute IsCurrentlyOpen for resto {RestoId}", resto.Id);
        //         resto.IsCurrentlyOpen = false;
        //     }
        // }
        //
        // var response = new RestoResponse.Index { Restos = paged };
        // return Result.Success(response);
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
