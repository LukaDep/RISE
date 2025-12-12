using Rise.Domain.Notifications;
using Rise.Shared.Notifications;

namespace Rise.Domain.Tests.Notifications;

public class SentNotificationShould
{
    [Fact]
    public void RequireTitleAndBody()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test Notification",
            Body = "This is a test notification body"
        };

        // Assert
        Assert.Equal("Test Notification", notification.Title);
        Assert.Equal("This is a test notification body", notification.Body);
    }

    [Fact]
    public void HaveDefaultDeliveryStatusPending()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body"
        };

        // Assert
        Assert.Equal(DeliveryStatus.Pending, notification.DeliveryStatus);
    }

    [Fact]
    public void HaveDefaultIsReadFalse()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body"
        };

        // Assert
        Assert.False(notification.IsRead);
    }

    [Fact]
    public void AllowUrlToBeNull()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body",
            Url = null
        };

        // Assert
        Assert.Null(notification.Url);
    }

    [Fact]
    public void AllowUrlToBeSet()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body",
            Url = "/grades/123"
        };

        // Assert
        Assert.Equal("/grades/123", notification.Url);
    }

    [Fact]
    public void AllowNotificationTypeToBeSet()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "New Grade",
            Body = "You received a grade",
            NotificationType = "grades"
        };

        // Assert
        Assert.Equal("grades", notification.NotificationType);
    }

    [Theory]
    [InlineData("grades")]
    [InlineData("schedule")]
    [InlineData("campus")]
    [InlineData("news")]
    public void SupportAllNotificationTypes(string notificationType)
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body",
            NotificationType = notificationType
        };

        // Assert
        Assert.Equal(notificationType, notification.NotificationType);
    }

    [Fact]
    public void TrackSentAt()
    {
        // Arrange
        var beforeCreation = DateTime.Now.AddSeconds(-1);

        // Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body"
        };

        // Assert
        Assert.True(notification.SentAt >= beforeCreation);
    }

    [Fact]
    public void AllowReadAtToBeNull()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body"
        };

        // Assert
        Assert.Null(notification.ReadAt);
    }

    [Fact]
    public void TrackReadAt()
    {
        // Arrange
        var readTime = DateTime.Now;

        // Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body",
            IsRead = true,
            ReadAt = readTime
        };

        // Assert
        Assert.True(notification.IsRead);
        Assert.Equal(readTime, notification.ReadAt);
    }

    [Fact]
    public void TrackPushSubscriptionId()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();

        // Act
        var notification = new SentNotification
        {
            PushSubscriptionId = subscriptionId,
            Title = "Test",
            Body = "Test body"
        };

        // Assert
        Assert.Equal(subscriptionId, notification.PushSubscriptionId);
    }

    [Fact]
    public void AllowUserIdToBeNull()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body",
            UserId = null
        };

        // Assert
        Assert.Null(notification.UserId);
    }

    [Fact]
    public void TrackUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body",
            UserId = userId
        };

        // Assert
        Assert.Equal(userId, notification.UserId);
    }

    [Theory]
    [InlineData(DeliveryStatus.Pending)]
    [InlineData(DeliveryStatus.Delivered)]
    [InlineData(DeliveryStatus.Failed)]
    [InlineData(DeliveryStatus.NoSubscription)]
    public void SupportAllDeliveryStatuses(DeliveryStatus status)
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body",
            DeliveryStatus = status
        };

        // Assert
        Assert.Equal(status, notification.DeliveryStatus);
    }

    [Fact]
    public void AllowPushSubscriptionNavigationProperty()
    {
        // Arrange
        var subscription = new PushSubscriptions
        {
            Endpoint = "https://example.com/push",
            P256dhKey = "test-key",
            AuthKey = "test-auth"
        };

        // Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body",
            PushSubscriptionId = subscription.Id,
            PushSubscription = subscription
        };

        // Assert
        Assert.NotNull(notification.PushSubscription);
        Assert.Equal(subscription.Id, notification.PushSubscription.Id);
    }

    [Fact]
    public void InheritFromEntity()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            Title = "Test",
            Body = "Test body"
        };

        // Assert - Entity properties are accessible
        Assert.NotEqual(default, notification.Id);
    }
}
