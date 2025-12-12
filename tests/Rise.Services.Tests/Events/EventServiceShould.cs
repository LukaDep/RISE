using Rise.Domain.Events;
using Rise.Services.Events;
using Rise.Services.Tests.TestInfrastructure;
using Rise.Shared.Common;

namespace Rise.Services.Tests.Events;

public class EventServiceShould
{
    [Fact]
    public async Task GetIndexAsync_ReturnsEmptyList_WhenNoEventsExist()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        var service = new EventService(dbContext);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            Filters = new Dictionary<string, object?>()
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Event.ShouldNotBeNull();
        result.Value.Event.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsFutureEvents_Only()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var futureEvent = new Event
        {
            Title = "Future Event",
            Type = "Academisch",
            Location = "Campus A",
            StartDateTime = DateTime.UtcNow.AddDays(5),
            EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(2),
            Description = "A future event"
        };
        var pastEvent = new Event
        {
            Title = "Past Event",
            Type = "Sport",
            Location = "Campus B",
            StartDateTime = DateTime.UtcNow.AddDays(-5),
            EndDateTime = DateTime.UtcNow.AddDays(-5).AddHours(2),
            Description = "A past event"
        };

        dbContext.Events.AddRange(futureEvent, pastEvent);
        await dbContext.SaveChangesAsync();

        var service = new EventService(dbContext);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            Filters = new Dictionary<string, object?>()
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Event.Count().ShouldBe(1);
        result.Value.Event.First().Name.ShouldBe("Future Event");
    }

    [Fact]
    public async Task GetIndexAsync_FiltersEvents_ByType()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sportEvent = new Event
        {
            Title = "Basketball Game",
            Type = "Sport",
            Location = "Sports Hall",
            StartDateTime = DateTime.UtcNow.AddDays(3),
            EndDateTime = DateTime.UtcNow.AddDays(3).AddHours(2)
        };
        var culturalEvent = new Event
        {
            Title = "Art Exhibition",
            Type = "Cultuur",
            Location = "Gallery",
            StartDateTime = DateTime.UtcNow.AddDays(5),
            EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(3)
        };

        dbContext.Events.AddRange(sportEvent, culturalEvent);
        await dbContext.SaveChangesAsync();

        var service = new EventService(dbContext);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            Filters = new Dictionary<string, object?>
            {
                { "Type", "Sport" }
            }
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Event.Count().ShouldBe(1);
        result.Value.Event.First().Name.ShouldBe("Basketball Game");
    }

    [Fact]
    public async Task GetIndexAsync_SearchesEvents_ByTitleOrType()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var event1 = new Event
        {
            Title = "Basketball Championship",
            Type = "Sport",
            Location = "Sports Hall",
            StartDateTime = DateTime.UtcNow.AddDays(3),
            EndDateTime = DateTime.UtcNow.AddDays(3).AddHours(2)
        };
        var event2 = new Event
        {
            Title = "Art Exhibition",
            Type = "Cultuur",
            Location = "Gallery",
            StartDateTime = DateTime.UtcNow.AddDays(5),
            EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(3)
        };

        dbContext.Events.AddRange(event1, event2);
        await dbContext.SaveChangesAsync();

        var service = new EventService(dbContext);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            SearchTerm = "Basketball",
            Filters = new Dictionary<string, object?>()
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Event.Count().ShouldBe(1);
        result.Value.Event.First().Name.ShouldBe("Basketball Championship");
    }

    [Fact]
    public async Task GetIndexAsync_OrdersEvents_ByStartDateThenType()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var laterEvent = new Event
        {
            Title = "Later Event",
            Type = "Sport",
            Location = "Location A",
            StartDateTime = DateTime.UtcNow.AddDays(10),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2)
        };
        var earlierEvent = new Event
        {
            Title = "Earlier Event",
            Type = "Cultuur",
            Location = "Location B",
            StartDateTime = DateTime.UtcNow.AddDays(5),
            EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(2)
        };

        dbContext.Events.AddRange(laterEvent, earlierEvent);
        await dbContext.SaveChangesAsync();

        var service = new EventService(dbContext);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            Filters = new Dictionary<string, object?>()
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var events = result.Value.Event.ToList();
        events[0].Name.ShouldBe("Earlier Event");
        events[1].Name.ShouldBe("Later Event");
    }

    [Fact]
    public async Task GetIndexAsync_RespectsPagination_SkipAndTake()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        for (int i = 1; i <= 5; i++)
        {
            dbContext.Events.Add(new Event
            {
                Title = $"Event {i}",
                Type = "Academisch",
                Location = $"Location {i}",
                StartDateTime = DateTime.UtcNow.AddDays(i),
                EndDateTime = DateTime.UtcNow.AddDays(i).AddHours(2)
            });
        }
        await dbContext.SaveChangesAsync();

        var service = new EventService(dbContext);

        var request = new QueryRequest.SkipTake
        {
            Skip = 2,
            Take = 2,
            Filters = new Dictionary<string, object?>()
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Event.Count().ShouldBe(2);
        var events = result.Value.Event.ToList();
        events[0].Name.ShouldBe("Event 3");
        events[1].Name.ShouldBe("Event 4");
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsCorrectEventProperties()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var startDate = DateTime.UtcNow.AddDays(5);
        var endDate = startDate.AddHours(3);
        var testEvent = new Event
        {
            Title = "Test Event",
            Type = "Welzijn",
            Location = "Test Location",
            StartDateTime = startDate,
            EndDateTime = endDate,
            Description = "Test Description",
            RegistrationLink = "https://example.com/register"
        };

        dbContext.Events.Add(testEvent);
        await dbContext.SaveChangesAsync();

        var service = new EventService(dbContext);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            Filters = new Dictionary<string, object?>()
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var returnedEvent = result.Value.Event.First();
        returnedEvent.Name.ShouldBe("Test Event");
        returnedEvent.Type.ShouldBe("Welzijn");
        returnedEvent.Location.ShouldBe("Test Location");
        returnedEvent.Description.ShouldBe("Test Description");
        returnedEvent.RegistrationLink.ShouldBe("https://example.com/register");
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsTodaysEvents()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var todayEvent = new Event
        {
            Title = "Today's Event",
            Type = "Academisch",
            Location = "Campus",
            StartDateTime = DateTime.Today.AddHours(14).ToUniversalTime(), // Today at 2 PM
            EndDateTime = DateTime.Today.AddHours(16).ToUniversalTime()
        };

        dbContext.Events.Add(todayEvent);
        await dbContext.SaveChangesAsync();

        var service = new EventService(dbContext);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            Filters = new Dictionary<string, object?>()
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Event.Count().ShouldBe(1);
        result.Value.Event.First().Name.ShouldBe("Today's Event");
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsAllEvents_WhenNoTypeFilterProvided()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var event1 = new Event
        {
            Title = "Sport Event",
            Type = "Sport",
            Location = "Sports Hall",
            StartDateTime = DateTime.UtcNow.AddDays(3),
            EndDateTime = DateTime.UtcNow.AddDays(3).AddHours(2)
        };
        var event2 = new Event
        {
            Title = "Culture Event",
            Type = "Cultuur",
            Location = "Gallery",
            StartDateTime = DateTime.UtcNow.AddDays(5),
            EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(3)
        };
        var event3 = new Event
        {
            Title = "Wellness Event",
            Type = "Welzijn",
            Location = "Wellness Center",
            StartDateTime = DateTime.UtcNow.AddDays(7),
            EndDateTime = DateTime.UtcNow.AddDays(7).AddHours(2)
        };

        dbContext.Events.AddRange(event1, event2, event3);
        await dbContext.SaveChangesAsync();

        var service = new EventService(dbContext);

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
            Filters = new Dictionary<string, object?>
            {
                { "Type", "" } // Empty filter should return all
            }
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Event.Count().ShouldBe(3);
    }
}
