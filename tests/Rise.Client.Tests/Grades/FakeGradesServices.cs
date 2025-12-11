namespace Rise.Client.Tests.Grades;

using Rise.Shared.Common;
using Rise.Shared.Grades;
using Ardalis.Result;

public class NullGradesService : IGradesService
{
    public Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var wrapper = new GradesResponse.Index
        {
            Grades = Enumerable.Empty<GradesDto.Grade>()
        };

        return Task.FromResult(Result.Success(wrapper));
    }

    public Task<Result<GradesResponse.Get>> GetGradeByIdAsync(Guid id, CancellationToken ctx = default)
    {
        return Task.FromResult(Result<GradesResponse.Get>.NotFound($"Grade item with id {id} not found."));
    }
}

public class FakeGradesService : IGradesService
{
    private readonly List<GradesDto.Grade> _items = new()
        {
            new GradesDto.Grade { Id = Guid.CreateVersion7(), CourseName = "Mathematics", CourseId = "Course1", Year = "2025", Semester = 1, Name = "Midterm Exam", ActivityType = "Exam", MaxPoints = 100, Score = 85, Date = DateTime.Now.AddDays(-30) },
            new GradesDto.Grade { Id = Guid.CreateVersion7(), CourseName = "Physics", CourseId = "Course2", Year = "2025", Semester = 1, Name = "Final Exam", ActivityType = "Exam", MaxPoints = 100, Score = 90, Date = DateTime.Now.AddDays(-5) },
            new GradesDto.Grade { Id = Guid.CreateVersion7(), CourseName = "Chemistry", CourseId = "Course3", Year = "2024", Semester = 2, Name = "Lab Work", ActivityType = "Lab", MaxPoints = 100, Score = 78, Date = DateTime.Now.AddDays(-200) },
            new GradesDto.Grade { Id = Guid.CreateVersion7(), CourseName = "Biology", CourseId = "Course4", Year = "2025", Semester = 1, Name = "Project", ActivityType = "Project", MaxPoints = 100, Score = 88, Date = DateTime.Now },
        };

    public Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = _items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request?.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(n => n.CourseName.Contains(term, StringComparison.OrdinalIgnoreCase)
                         || n.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        query = query.OrderByDescending(n => n.Date);

        var skip = Math.Max(0, request?.Skip ?? 0);
        var take = Math.Max(0, request?.Take ?? 20);

        var page = query.Skip(skip).Take(take).ToList();

        var wrapper = new GradesResponse.Index
        {
            Grades = page
        };
        return Task.FromResult(Result.Success(wrapper));
    }

    public Task<Result<GradesResponse.Get>> GetGradeByIdAsync(Guid id, CancellationToken ctx = default)
    {
        var item = _items.FirstOrDefault(g => g.Id == id);
        if (item == null)
        {
            return Task.FromResult(Result<GradesResponse.Get>.NotFound($"Grade item with id {id} not found."));
        }

        var wrapper = new GradesResponse.Get
        {
            Grade = item
        };
        return Task.FromResult(Result.Success(wrapper));
    }
}
