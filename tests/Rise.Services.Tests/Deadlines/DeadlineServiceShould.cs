using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Rise.Domain.Deadlines;
using Rise.Persistence;
using Rise.Services.Deadlines;
using Rise.Services.Tests.Fakers;
using Rise.Services.Tests.TestInfrastructure;
using Rise.Shared.Common;

namespace Rise.Services.Tests.Deadlines;

public class DeadlineServiceShould
{
    private const string TestUserId = "11111111-1111-1111-1111-111111111111";
    private const string OtherUserId = "22222222-2222-2222-2222-222222222222";

    private static ClaimsPrincipal CreateTestUser(string userId = TestUserId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    /// <summary>
    /// Creates an IdentityUser in the database to satisfy FK constraints.
    /// </summary>
    private static async Task CreateIdentityUserAsync(ApplicationDbContext dbContext, string userId)
    {
        var user = new IdentityUser
        {
            Id = userId,
            UserName = $"testuser_{userId}",
            NormalizedUserName = $"TESTUSER_{userId}",
            Email = $"test_{userId}@example.com",
            NormalizedEmail = $"TEST_{userId}@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsEmptyList_WhenNoDeadlinesExist()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        var sessionProvider = new FakeSessionContextProvider(CreateTestUser());
        var service = new DeadlineService(dbContext, sessionProvider);

        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Deadlines.ShouldNotBeNull();
        result.Value.Deadlines.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsDeadlines_ForCurrentUser()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        await CreateIdentityUserAsync(dbContext, TestUserId);

        var deadline1 = new Deadline
        {
            UserId = TestUserId,
            Title = "Assignment 1",
            Lector = "Prof. Smith",
            EndDate = DateTime.UtcNow.AddDays(5),
            Description = "Complete the first assignment",
            Course = "Mathematics"
        };
        var deadline2 = new Deadline
        {
            UserId = TestUserId,
            Title = "Project Deadline",
            Lector = "Prof. Johnson",
            EndDate = DateTime.UtcNow.AddDays(10),
            Description = "Submit final project",
            Course = "Programming"
        };

        dbContext.Deadlines.AddRange(deadline1, deadline2);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateTestUser());
        var service = new DeadlineService(dbContext, sessionProvider);

        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Deadlines.Count().ShouldBe(2);
    }

    [Fact]
    public async Task GetIndexAsync_DoesNotReturnDeadlines_ForOtherUsers()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        await CreateIdentityUserAsync(dbContext, OtherUserId);

        var deadline = new Deadline
        {
            UserId = OtherUserId,
            Title = "Other User's Deadline",
            Lector = "Prof. Wilson",
            EndDate = DateTime.UtcNow.AddDays(5),
            Course = "History"
        };

        dbContext.Deadlines.Add(deadline);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateTestUser(TestUserId));
        var service = new DeadlineService(dbContext, sessionProvider);

        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Deadlines.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetIndexAsync_FiltersExpiredDeadlines_OlderThanOneWeek()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        await CreateIdentityUserAsync(dbContext, TestUserId);

        var recentDeadline = new Deadline
        {
            UserId = TestUserId,
            Title = "Recent Deadline",
            Lector = "Prof. Smith",
            EndDate = DateTime.UtcNow.AddDays(-3), // 3 days ago - should be included
            Course = "Math"
        };
        var oldDeadline = new Deadline
        {
            UserId = TestUserId,
            Title = "Old Deadline",
            Lector = "Prof. Johnson",
            EndDate = DateTime.UtcNow.AddDays(-10), // 10 days ago - should be filtered out
            Course = "Science"
        };

        dbContext.Deadlines.AddRange(recentDeadline, oldDeadline);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateTestUser());
        var service = new DeadlineService(dbContext, sessionProvider);

        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Deadlines.Count().ShouldBe(1);
        result.Value.Deadlines.First().Title.ShouldBe("Recent Deadline");
    }

    [Fact]
    public async Task GetIndexAsync_FiltersDeadlines_ByDateRange()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        await CreateIdentityUserAsync(dbContext, TestUserId);

        var deadline1 = new Deadline
        {
            UserId = TestUserId,
            Title = "Deadline In Range",
            Lector = "Prof. Smith",
            EndDate = DateTime.UtcNow.AddDays(5),
            Course = "Math"
        };
        var deadline2 = new Deadline
        {
            UserId = TestUserId,
            Title = "Deadline Outside Range",
            Lector = "Prof. Johnson",
            EndDate = DateTime.UtcNow.AddDays(20),
            Course = "Science"
        };

        dbContext.Deadlines.AddRange(deadline1, deadline2);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateTestUser());
        var service = new DeadlineService(dbContext, sessionProvider);

        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Deadlines.Count().ShouldBe(1);
        result.Value.Deadlines.First().Title.ShouldBe("Deadline In Range");
    }

    [Fact]
    public async Task GetIndexAsync_OrdersDeadlines_ByEndDateAscending()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        await CreateIdentityUserAsync(dbContext, TestUserId);

        var laterDeadline = new Deadline
        {
            UserId = TestUserId,
            Title = "Later Deadline",
            Lector = "Prof. Smith",
            EndDate = DateTime.UtcNow.AddDays(10),
            Course = "Math"
        };
        var earlierDeadline = new Deadline
        {
            UserId = TestUserId,
            Title = "Earlier Deadline",
            Lector = "Prof. Johnson",
            EndDate = DateTime.UtcNow.AddDays(3),
            Course = "Science"
        };

        dbContext.Deadlines.AddRange(laterDeadline, earlierDeadline);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateTestUser());
        var service = new DeadlineService(dbContext, sessionProvider);

        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var deadlines = result.Value.Deadlines.ToList();
        deadlines[0].Title.ShouldBe("Earlier Deadline");
        deadlines[1].Title.ShouldBe("Later Deadline");
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsEmptyList_WhenUserNotAuthenticated()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        await CreateIdentityUserAsync(dbContext, TestUserId);

        var deadline = new Deadline
        {
            UserId = TestUserId,
            Title = "Some Deadline",
            Lector = "Prof. Smith",
            EndDate = DateTime.UtcNow.AddDays(5),
            Course = "Math"
        };

        dbContext.Deadlines.Add(deadline);
        await dbContext.SaveChangesAsync();

        // Create session provider with no user claims (empty user)
        var emptyUser = new ClaimsPrincipal(new ClaimsIdentity());
        var sessionProvider = new FakeSessionContextProvider(emptyUser);
        var service = new DeadlineService(dbContext, sessionProvider);

        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Deadlines.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetIndexAsync_ExcludesDeletedDeadlines()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        await CreateIdentityUserAsync(dbContext, TestUserId);

        var activeDeadline = new Deadline
        {
            UserId = TestUserId,
            Title = "Active Deadline",
            Lector = "Prof. Smith",
            EndDate = DateTime.UtcNow.AddDays(5),
            Course = "Math"
        };
        var deletedDeadline = new Deadline
        {
            UserId = TestUserId,
            Title = "Deleted Deadline",
            Lector = "Prof. Johnson",
            EndDate = DateTime.UtcNow.AddDays(5),
            Course = "Science",
            IsDeleted = true
        };

        dbContext.Deadlines.AddRange(activeDeadline, deletedDeadline);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateTestUser());
        var service = new DeadlineService(dbContext, sessionProvider);

        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Deadlines.Count().ShouldBe(1);
        result.Value.Deadlines.First().Title.ShouldBe("Active Deadline");
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsCorrectDeadlineProperties()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        await CreateIdentityUserAsync(dbContext, TestUserId);

        var endDate = DateTime.UtcNow.AddDays(5);
        var deadline = new Deadline
        {
            UserId = TestUserId,
            Title = "Test Assignment",
            Lector = "Prof. Test",
            EndDate = endDate,
            Description = "Test description",
            Course = "Test Course"
        };

        dbContext.Deadlines.Add(deadline);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateTestUser());
        var service = new DeadlineService(dbContext, sessionProvider);

        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10
        };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var returnedDeadline = result.Value.Deadlines.First();
        returnedDeadline.Title.ShouldBe("Test Assignment");
        returnedDeadline.Lector.ShouldBe("Prof. Test");
        returnedDeadline.Description.ShouldBe("Test description");
        returnedDeadline.Course.ShouldBe("Test Course");
        returnedDeadline.UserId.ShouldBe(TestUserId);
    }
}
