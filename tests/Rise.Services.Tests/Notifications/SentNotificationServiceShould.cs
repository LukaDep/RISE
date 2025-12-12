using System.Security.Claims;
using Rise.Domain.Notifications;
using Rise.Persistence;
using Rise.Services.Notifications;
using Rise.Services.Tests.Fakers;
using Rise.Services.Tests.TestInfrastructure;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Rise.Services.Tests.Notifications;

public class SentNotificationServiceShould
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

    private static async Task<SentNotification> CreateSentNotificationAsync(
        ApplicationDbContext dbContext,
        Guid subscriptionId,
        Guid userId,
        string title = "Test Notification",
        string body = "Test body",
        bool isRead = false,
        bool isDeleted = false)
    {
        var notification = new SentNotification
        {
            PushSubscriptionId = subscriptionId,
            UserId = userId,
            Title = title,
            Body = body,
            IsRead = isRead,
            IsDeleted = isDeleted,
            SentAt = DateTime.Now
        };
        dbContext.SentNotifications.Add(notification);
        await dbContext.SaveChangesAsync();
        return notification;
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithValidUser_ShouldReturnNotifications()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Notification 1", "Body 1");
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Notification 2", "Body 2");

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.GetUserNotificationsAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithUnauthenticatedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(new ClaimsPrincipal());
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.GetUserNotificationsAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldExcludeDeletedNotifications()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Visible", "Body", isDeleted: false);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Deleted", "Body", isDeleted: true);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.GetUserNotificationsAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Notifications);
        Assert.Equal("Visible", result.Value.Notifications.First().Title);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnUnreadCount()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Unread 1", "Body", isRead: false);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Unread 2", "Body", isRead: false);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Read", "Body", isRead: true);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.GetUserNotificationsAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value!.TotalCount);
        Assert.Equal(2, result.Value.UnreadCount);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldSupportPagination()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);
        
        for (int i = 0; i < 25; i++)
        {
            await CreateSentNotificationAsync(dbContext, subscription.Id, userId, $"Notification {i}", "Body");
        }

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.GetUserNotificationsAsync(page: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(25, result.Value!.TotalCount);
        Assert.Equal(10, result.Value.Notifications.Count());
    }

    [Fact]
    public async Task MarkAsReadAsync_WithValidNotification_ShouldMarkAsRead()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);
        var notification = await CreateSentNotificationAsync(dbContext, subscription.Id, userId, isRead: false);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.MarkAsReadAsync(new SentNotificationRequest.MarkAsRead
        {
            NotificationId = notification.Id
        });

        // Assert
        Assert.True(result.IsSuccess);

        var updated = await dbContext.SentNotifications.FindAsync(notification.Id);
        Assert.True(updated!.IsRead);
        Assert.NotNull(updated.ReadAt);
    }

    [Fact]
    public async Task MarkAsReadAsync_WithUnauthenticatedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(new ClaimsPrincipal());
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.MarkAsReadAsync(new SentNotificationRequest.MarkAsRead
        {
            NotificationId = Guid.NewGuid()
        });

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task MarkAsReadAsync_WithNonExistentNotification_ShouldReturnNotFound()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.MarkAsReadAsync(new SentNotificationRequest.MarkAsRead
        {
            NotificationId = Guid.NewGuid()
        });

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task MarkAsReadAsync_WithAlreadyReadNotification_ShouldNotUpdateReadAt()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);
        
        var originalReadAt = DateTime.Now.AddDays(-1);
        var notification = new SentNotification
        {
            PushSubscriptionId = subscription.Id,
            UserId = userId,
            Title = "Test",
            Body = "Test body",
            IsRead = true,
            ReadAt = originalReadAt,
            SentAt = DateTime.Now.AddDays(-2)
        };
        dbContext.SentNotifications.Add(notification);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.MarkAsReadAsync(new SentNotificationRequest.MarkAsRead
        {
            NotificationId = notification.Id
        });

        // Assert
        Assert.True(result.IsSuccess);
        
        var updated = await dbContext.SentNotifications.FindAsync(notification.Id);
        Assert.Equal(originalReadAt, updated!.ReadAt);
    }

    [Fact]
    public async Task MarkAllAsReadAsync_ShouldMarkAllUnreadNotifications()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);
        var n1 = await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Unread 1", "Body", isRead: false);
        var n2 = await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Unread 2", "Body", isRead: false);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Already read", "Body", isRead: true);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.MarkAllAsReadAsync();

        // Assert
        Assert.True(result.IsSuccess);

        // Reload the notifications from the database to get updated values
        using var verifyContext = fixture.CreateContext();
        var notifications = await verifyContext.SentNotifications.Where(n => n.UserId == userId).ToListAsync();
        Assert.All(notifications, n => Assert.True(n.IsRead));
    }

    [Fact]
    public async Task MarkAllAsReadAsync_WithUnauthenticatedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(new ClaimsPrincipal());
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.MarkAllAsReadAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetUnreadCountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Unread 1", "Body", isRead: false);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Unread 2", "Body", isRead: false);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Read", "Body", isRead: true);
        await CreateSentNotificationAsync(dbContext, subscription.Id, userId, "Deleted", "Body", isRead: false, isDeleted: true);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.GetUnreadCountAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public async Task GetUnreadCountAsync_WithUnauthenticatedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(new ClaimsPrincipal());
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.GetUnreadCountAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteNotificationAsync_ShouldSoftDelete()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);
        var notification = await CreateSentNotificationAsync(dbContext, subscription.Id, userId);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.DeleteNotificationAsync(notification.Id);

        // Assert
        Assert.True(result.IsSuccess);

        var deleted = await dbContext.SentNotifications.FindAsync(notification.Id);
        Assert.True(deleted!.IsDeleted);
    }

    [Fact]
    public async Task DeleteNotificationAsync_WithNonExistentNotification_ShouldReturnNotFound()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.DeleteNotificationAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteNotificationAsync_WithOtherUsersNotification_ShouldReturnNotFound()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId1.ToString());
        await CreateIdentityUserAsync(dbContext, userId2.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId1);
        var notification = await CreateSentNotificationAsync(dbContext, subscription.Id, userId1);

        // Try to delete as different user
        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId2.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        var result = await service.DeleteNotificationAsync(notification.Id);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task SaveSentNotificationAsync_ShouldSaveNotification()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid();
        await CreateIdentityUserAsync(dbContext, userId.ToString());
        var subscription = await CreatePushSubscriptionAsync(dbContext, userId);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId.ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        await service.SaveSentNotificationAsync(
            subscription.Id,
            "Test Title",
            "Test Body",
            "/test/url",
            "grades",
            DeliveryStatus.Delivered
        );

        // Assert
        var saved = await dbContext.SentNotifications.FirstOrDefaultAsync(n => n.Title == "Test Title");
        Assert.NotNull(saved);
        Assert.Equal("Test Body", saved.Body);
        Assert.Equal("/test/url", saved.Url);
        Assert.Equal("grades", saved.NotificationType);
        Assert.Equal(DeliveryStatus.Delivered, saved.DeliveryStatus);
        Assert.Equal(userId, saved.UserId);
    }

    [Fact]
    public async Task SaveSentNotificationAsync_WithInvalidSubscription_ShouldNotSave()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(Guid.NewGuid().ToString()));
        var service = new SentNotificationService(dbContext, sessionProvider);

        // Act
        await service.SaveSentNotificationAsync(
            Guid.NewGuid(), // Non-existent subscription
            "Test Title",
            "Test Body"
        );

        // Assert
        var saved = await dbContext.SentNotifications.FirstOrDefaultAsync(n => n.Title == "Test Title");
        Assert.Null(saved);
    }
}
