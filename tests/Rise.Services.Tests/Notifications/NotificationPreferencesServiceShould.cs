using System.Security.Claims;
using Rise.Domain.Notifications;
using Rise.Persistence;
using Rise.Services.Notifications;
using Rise.Services.Tests.Fakers;
using Rise.Services.Tests.TestInfrastructure;
using Rise.Shared.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Rise.Services.Tests.Notifications;

public class NotificationPreferencesServiceShould
{
    private static ClaimsPrincipal CreateUserPrincipal(string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    private static async Task CreateIdentityUserAsync(ApplicationDbContext dbContext, string userId)
    {
        var user = new IdentityUser
        {
            Id = userId,
            UserName = $"testuser_{userId}@example.com",
            NormalizedUserName = $"TESTUSER_{userId}@EXAMPLE.COM",
            Email = $"testuser_{userId}@example.com",
            NormalizedEmail = $"TESTUSER_{userId}@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    }

    private static async Task<NotificationPreferences> CreatePreferencesAsync(ApplicationDbContext dbContext, Guid userId)
    {
        var preferences = new NotificationPreferences(userId)
        {
            GradesNotifications = true,
            ScheduleNotifications = true,
            CampusNotifications = true,
            NewsNotifications = true
        };
        dbContext.NotificationPreferences.Add(preferences);
        await dbContext.SaveChangesAsync();
        return preferences;
    }

    private static async Task<PushSubscriptions> CreatePushSubscriptionAsync(ApplicationDbContext dbContext, Guid userId)
    {
        var subscription = new PushSubscriptions
        {
            UserId = userId,
            Endpoint = "https://fcm.googleapis.com/fcm/send/test-" + Guid.NewGuid(),
            P256dhKey = "test-p256dh-key",
            AuthKey = "test-auth-key"
        };
        dbContext.PushSubscriptions.Add(subscription);
        await dbContext.SaveChangesAsync();
        return subscription;
    }

    [Fact]
    public async Task GetUserPreferencesByIdAsync_WithExistingPreferences_ShouldReturnPreferences()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var preferences = await CreatePreferencesAsync(dbContext, userId);
        await CreatePushSubscriptionAsync(dbContext, userId);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        // Act
        var result = await service.GetUserPreferencesByIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value?.NotificationPreference);
        Assert.True(result.Value.NotificationPreference.GradesNotifications);
        Assert.True(result.Value.NotificationPreference.ScheduleNotifications);
        Assert.True(result.Value.NotificationPreference.CampusNotifications);
        Assert.True(result.Value.NotificationPreference.NewsNotifications);
    }

    [Fact]
    public async Task GetUserPreferencesByIdAsync_WithNoPreferences_ShouldCreateDefaults()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        // Act
        var result = await service.GetUserPreferencesByIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value?.NotificationPreference);
        
        // Verify defaults were created
        Assert.True(result.Value.NotificationPreference.GradesNotifications);
        Assert.True(result.Value.NotificationPreference.ScheduleNotifications);
        Assert.True(result.Value.NotificationPreference.CampusNotifications);
        Assert.True(result.Value.NotificationPreference.NewsNotifications);

        // Verify saved to database
        var savedPrefs = await dbContext.NotificationPreferences.FindAsync(userId);
        Assert.NotNull(savedPrefs);
    }

    [Fact]
    public async Task GetUserPreferencesByIdAsync_WithUnauthenticatedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(new ClaimsPrincipal());
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        // Act
        var result = await service.GetUserPreferencesByIdAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task EditAsync_WithValidRequest_ShouldUpdatePreferences()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        await CreatePreferencesAsync(dbContext, userId);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        var editRequest = new NotificationPreferencesRequest.Edit
        {
            GradesNotifications = false,
            ScheduleNotifications = true,
            CampusNotifications = false,
            NewsNotifications = true
        };

        // Act
        var result = await service.EditAsync(editRequest, default);

        // Assert
        Assert.True(result.IsSuccess);

        var updated = await dbContext.NotificationPreferences.FindAsync(userId);
        Assert.False(updated!.GradesNotifications);
        Assert.True(updated.ScheduleNotifications);
        Assert.False(updated.CampusNotifications);
        Assert.True(updated.NewsNotifications);
    }

    [Fact]
    public async Task EditAsync_WithUnauthenticatedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(new ClaimsPrincipal());
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        // Act
        var result = await service.EditAsync(new NotificationPreferencesRequest.Edit(), default);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task EditAsync_WithNoExistingPreferences_ShouldReturnNotFound()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        // No preferences created

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        // Act
        var result = await service.EditAsync(new NotificationPreferencesRequest.Edit(), default);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Subscribe_WithNewSubscription_ShouldCreateSubscription()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        var request = new PushSubscriptionRequest.Create
        {
            Endpoint = "https://fcm.googleapis.com/fcm/send/new-endpoint",
            Keys = new PushSubscriptionRequest.Keys
            {
                P256dh = "new-p256dh-key",
                Auth = "new-auth-key"
            }
        };

        // Act
        var result = await service.Subscribe(request);

        // Assert
        Assert.True(result.IsSuccess);

        var saved = await dbContext.PushSubscriptions.FirstOrDefaultAsync(s => s.Endpoint == request.Endpoint);
        Assert.NotNull(saved);
        Assert.Equal(userId, saved.UserId);
        Assert.Equal("new-p256dh-key", saved.P256dhKey);
        Assert.Equal("new-auth-key", saved.AuthKey);
    }

    [Fact]
    public async Task Subscribe_WithExistingEndpoint_ShouldUpdateSubscription()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        
        var existingEndpoint = "https://fcm.googleapis.com/fcm/send/existing-endpoint";
        var existingSub = new PushSubscriptions
        {
            UserId = null, // Anonymous
            Endpoint = existingEndpoint,
            P256dhKey = "old-key",
            AuthKey = "old-auth"
        };
        dbContext.PushSubscriptions.Add(existingSub);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        var request = new PushSubscriptionRequest.Create
        {
            Endpoint = existingEndpoint,
            Keys = new PushSubscriptionRequest.Keys
            {
                P256dh = "updated-p256dh-key",
                Auth = "updated-auth-key"
            }
        };

        // Act
        var result = await service.Subscribe(request);

        // Assert
        Assert.True(result.IsSuccess);

        var subscriptions = await dbContext.PushSubscriptions.Where(s => s.Endpoint == existingEndpoint).ToListAsync();
        Assert.Single(subscriptions);
        Assert.Equal(userId, subscriptions.First().UserId);
        Assert.Equal("updated-p256dh-key", subscriptions.First().P256dhKey);
    }

    [Fact]
    public async Task Unsubscribe_WithExistingSubscription_ShouldDeleteSubscription()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        await CreatePushSubscriptionAsync(dbContext, userId);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        // Act
        var result = await service.Unsubscribe();

        // Assert
        Assert.True(result.IsSuccess);

        var remaining = await dbContext.PushSubscriptions.Where(s => s.UserId == userId).ToListAsync();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task Unsubscribe_WithUnauthenticatedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(new ClaimsPrincipal());
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        // Act
        var result = await service.Unsubscribe();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Unsubscribe_WithNoSubscription_ShouldSucceed()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        // No subscription created

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        // Act
        var result = await service.Unsubscribe();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendTestToUser_WithNoSubscription_ShouldReturnError()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        // No subscription

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var sentNotificationService = Substitute.For<ISentNotificationService>();
        var service = new NotificationPreferencesService(dbContext, sessionProvider, sentNotificationService);

        var request = new Push.Send
        {
            userGuid = userId,
            title = "Test",
            body = "Test body"
        };

        // Act
        var result = await service.SendTestToUser(request);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
