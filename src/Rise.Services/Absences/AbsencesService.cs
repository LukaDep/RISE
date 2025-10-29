using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Shared.Absences;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Services.Absences;

public class AbsencesService : IAbsencesService
{
    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Rise.Services", "Absences", "MockData", "mocks.json");

    public async Task<Result<AbsencesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        //read from mock json file
        if (!File.Exists(_mockFilePath))
            return Result<AbsencesResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");
        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);

        // Deserialize the JSON data into a list of NewsDto.Index
        var query = JsonSerializer.Deserialize<List<AbsenceDto.Index>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        // Apply ordering
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy)).ToList()
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy)).ToList();
        }
        else
        {
            // Default order
            query = query.OrderBy(p => p.StartDate).ToList();
        }

        var absences = query
            .Skip(request.Skip)
            .Take(request.Take);

        return Result.Success(new AbsencesResponse.Index
            {
                Absences = absences,
            }
        );
    }
}