using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Absences;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Services.Absences;

public class AbsencesService(ApplicationDbContext dbContext) : IAbsencesService
{
    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Rise.Services", "Absences", "MockData", "mocks.json");

    public async Task<Result<AbsencesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        // //read from mock json file
        // if (!File.Exists(_mockFilePath))
        //     return Result<AbsencesResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");
        // var json = await File.ReadAllTextAsync(_mockFilePath, ctx);

        // Deserialize the JSON data into a list of NewsDto.Index
        // var query = JsonSerializer.Deserialize<List<AbsenceDto.Index>>(json,
        //     new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

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