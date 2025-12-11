using Ardalis.Result;
using Rise.Shared.Common;
using Rise.Shared.Grades;

namespace Rise.Client.Tests.Widgets;

public class NullGradesService : IGradesService
{
    public Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var wrapper = new GradesResponse.Index
        {
            Grades = new List<GradesDto.Grade>()
        };
        return Task.FromResult(Result.Success(wrapper));
    }

    public Task<Result<GradesResponse.Get>> GetGradeByIdAsync(Guid id, CancellationToken ctx = default)
    {
        return Task.FromResult(Result<GradesResponse.Get>.NotFound($"Grade with id {id} not found."));
    }
}

public class FakeGradesService : IGradesService
{
    private readonly List<GradesDto.Grade> _items = new()
    {
        new GradesDto.Grade 
        { 
            Id = Guid.NewGuid(), 
            Name = "Mathematics", 
            Score = 15, 
            MaxPoints = 20, 
            Date = DateTime.UtcNow.AddDays(-1),
            ActivityType = "Exam"
        },
        new GradesDto.Grade 
        { 
            Id = Guid.NewGuid(), 
            Name = "Physics", 
            Score = 18, 
            MaxPoints = 20, 
            Date = DateTime.UtcNow.AddDays(-2),
            ActivityType = "Exam"
        },
        new GradesDto.Grade 
        { 
            Id = Guid.NewGuid(), 
            Name = "Chemistry", 
            Score = 16, 
            MaxPoints = 20, 
            Date = DateTime.UtcNow.AddDays(-3),
            ActivityType = "Exam"
        }
    };

    public Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = _items.AsEnumerable();
        
        if (!string.IsNullOrEmpty(request.OrderBy))
        {
            query = request.OrderBy.Contains("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(g => g.Date)
                : query.OrderBy(g => g.Date);
        }

        var page = query.Skip(0).Take(100).ToList();

        var wrapper = new GradesResponse.Index
        {
            Grades = page
        };

        return Task.FromResult(Result.Success(wrapper));
    }

    public Task<Result<GradesResponse.Get>> GetGradeByIdAsync(Guid id, CancellationToken ctx = default)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return Task.FromResult(Result<GradesResponse.Get>.NotFound($"Grade with id {id} not found."));

        var wrapper = new GradesResponse.Get
        {
            Grade = item
        };

        return Task.FromResult(Result.Success(wrapper));
    }
}

