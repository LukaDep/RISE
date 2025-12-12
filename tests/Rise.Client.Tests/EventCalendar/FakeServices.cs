using Ardalis.Result;
using Rise.Shared.Common;
using Rise.Shared.Events;

namespace Rise.Client.EventCalendar;

public class FakeEventService : IEventService
{
    private readonly List<EventDTO.Index> _events = new()
    {
        new EventDTO.Index
        {
            Type = "Sport",
            Name = "Basketball Game",
            StartDateTime = DateTime.UtcNow.AddDays(3),
            EndDateTime = DateTime.UtcNow.AddDays(3).AddHours(2),
            Location = "Sports Hall",
            Description = "Annual basketball tournament",
            RegistrationLink = "https://example.com/register/basketball"
        },
        new EventDTO.Index
        {
            Type = "Cultuur",
            Name = "Art Exhibition",
            StartDateTime = DateTime.UtcNow.AddDays(5),
            EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(4),
            Location = "Art Gallery",
            Description = "Contemporary art exhibition featuring local artists"
        },
        new EventDTO.Index
        {
            Type = "Welzijn",
            Name = "Yoga Session",
            StartDateTime = DateTime.UtcNow.AddDays(7),
            EndDateTime = DateTime.UtcNow.AddDays(7).AddHours(1),
            Location = "Wellness Center",
            Description = "Relaxing yoga session for students"
        },
        new EventDTO.Index
        {
            Type = "Academisch",
            Name = "Guest Lecture",
            StartDateTime = DateTime.UtcNow.AddDays(10),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            Location = "Auditorium A",
            Description = "Guest lecture on AI and Machine Learning"
        }
    };

    public Task<Result<EventResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = _events.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(e => e.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                                  || e.Type.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (request.Filters.TryGetValue("Type", out var typeFilter) && !string.IsNullOrWhiteSpace(typeFilter?.ToString()))
        {
            query = query.Where(e => e.Type.Equals(typeFilter.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        query = query.OrderBy(e => e.StartDateTime);

        var page = query.Skip(request.Skip).Take(request.Take).ToList();

        var response = new EventResponse.Index
        {
            Event = page
        };

        return Task.FromResult(Result.Success(response));
    }
}

public class EmptyEventService : IEventService
{
    public Task<Result<EventResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var response = new EventResponse.Index
        {
            Event = new List<EventDTO.Index>()
        };

        return Task.FromResult(Result.Success(response));
    }
}

public class NullEventService : IEventService
{
    public Task<Result<EventResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var response = new EventResponse.Index
        {
            Event = null!
        };

        return Task.FromResult(Result.Success(response));
    }
}
