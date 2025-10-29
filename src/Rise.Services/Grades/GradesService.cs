namespace Rise.Services.Grades;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// Service for grades.
/// </summary>

public class GradesService() : IGradesService
{
    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Rise.Services", "Grades", "MockData", "grades.json");
    public async Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        // mockdata
        if (!File.Exists(_mockFilePath))
            return Result<GradesResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");
        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        // deserialize
        var query = JsonSerializer.Deserialize<List<GradesDto.Grade>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(g => (g.CourseName ?? string.Empty).Contains(request.SearchTerm, StringComparison.CurrentCultureIgnoreCase)
                                     || (g.Name ?? string.Empty).Contains(request.SearchTerm, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy)).ToList()
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy)).ToList();
        }
        else
        {
            query = query.OrderByDescending(g => g.Date).ToList();
        }

        var grades = query.Skip(request.Skip).Take(request.Take).ToList();
        var response = new GradesResponse.Index { Grades = grades };
        return Result.Success(response);
    }

    public async Task<Result<GradesResponse.GradeById>> GetGradeByIdAsync(string id, CancellationToken ctx)
    {
        if (!File.Exists(_mockFilePath))
            return Result<GradesResponse.GradeById>.NotFound($"Mock data file not found at: {_mockFilePath}");

        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        var items = JsonSerializer.Deserialize<List<GradesDto.Grade>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        var grade = items.FirstOrDefault(c => c.Id == id);
        if (grade == null)
        {
            return Result<GradesResponse.GradeById>.NotFound($"Mock data file not found at: {_mockFilePath}");
        }
        return Result.Success(new GradesResponse.GradeById { Grade = grade });
    }
}