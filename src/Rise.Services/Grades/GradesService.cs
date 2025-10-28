namespace Rise.Services.Grades;

using System.Text.Json;
using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// Service for grades.
/// </summary>

public class GradesService() : IGradesService
{
    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Rise.Services", "Grades", "MockData", "grades.json");
    public async Task<Result<GradesResponse.CourseList>> GetCoursesAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        if (!File.Exists(_mockFilePath))
            return Result<GradesResponse.CourseList>.NotFound($"Mock data file not found at: {_mockFilePath}");

        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        var items = JsonSerializer.Deserialize<List<GradesDto.Course>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        var response = new GradesResponse.CourseList { Courses = items };
        return Result.Success(response);
    }

    public async Task<Result<GradesResponse.CourseById>> GetCourseByIdAsync(string id, CancellationToken ctx)
    {
        if (!File.Exists(_mockFilePath))
            return Result<GradesResponse.CourseById>.NotFound($"Mock data file not found at: {_mockFilePath}");

        var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
        var items = JsonSerializer.Deserialize<List<GradesDto.Course>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        var course = items.FirstOrDefault(c => c.CourseId == id);
        if (course == null)
        {
            return Result<GradesResponse.CourseById>.NotFound($"Mock data file not found at: {_mockFilePath}");
        }
        return Result.Success(new GradesResponse.CourseById { Course = course });
    }
}