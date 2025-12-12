using Ardalis.Result;
using Rise.Shared.Common;
using Rise.Shared.Deadlines;

namespace Rise.Client.Deadlines;

public class FakeDeadlineService : IDeadlineService
{
    private readonly List<DeadlineDto.Index> _deadlines = new()
    {
        new DeadlineDto.Index
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            UserId = "11111111-1111-1111-1111-111111111111",
            Title = "Assignment 1",
            Lector = "Prof. Smith",
            EndDate = DateTime.UtcNow.AddDays(5),
            Description = "Complete the first assignment",
            Course = "Mathematics"
        },
        new DeadlineDto.Index
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            UserId = "11111111-1111-1111-1111-111111111111",
            Title = "Project Deadline",
            Lector = "Prof. Johnson",
            EndDate = DateTime.UtcNow.AddDays(10),
            Description = "Submit final project",
            Course = "Programming"
        },
        new DeadlineDto.Index
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            UserId = "11111111-1111-1111-1111-111111111111",
            Title = "Expired Assignment",
            Lector = "Prof. Wilson",
            EndDate = DateTime.UtcNow.AddDays(-2),
            Description = "An expired deadline",
            Course = "History"
        }
    };

    public Task<Result<DeadlineResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var query = _deadlines.AsEnumerable();

        if (request.StartDate.HasValue)
        {
            query = query.Where(d => d.EndDate.Date >= request.StartDate.Value.Date);
        }
        if (request.EndDate.HasValue)
        {
            query = query.Where(d => d.EndDate.Date <= request.EndDate.Value.Date);
        }

        query = query.OrderBy(d => d.EndDate);

        var page = query.Skip(request.Skip).Take(request.Take).ToList();

        var response = new DeadlineResponse.Index
        {
            Deadlines = page
        };

        return Task.FromResult(Result.Success(response));
    }
}

public class EmptyDeadlineService : IDeadlineService
{
    public Task<Result<DeadlineResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var response = new DeadlineResponse.Index
        {
            Deadlines = new List<DeadlineDto.Index>()
        };

        return Task.FromResult(Result.Success(response));
    }
}

public class NullDeadlineService : IDeadlineService
{
    public Task<Result<DeadlineResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var response = new DeadlineResponse.Index
        {
            Deadlines = null!
        };

        return Task.FromResult(Result.Success(response));
    }
}
