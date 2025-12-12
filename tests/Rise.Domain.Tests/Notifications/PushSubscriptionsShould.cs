using Rise.Domain.Notifications;

namespace Rise.Domain.Tests.Notifications;

public class PushSubscriptionsShould
{
    [Fact]
    public void RequireEndpoint()
    {
        // Arrange & Act
        var subscription = new PushSubscriptions
        {
            Endpoint = "https://fcm.googleapis.com/fcm/send/test-endpoint",
            P256dhKey = "test-p256dh-key",
            AuthKey = "test-auth-key"
        };

        // Assert
        Assert.Equal("https://fcm.googleapis.com/fcm/send/test-endpoint", subscription.Endpoint);
    }

    [Fact]
    public void RequireP256dhKey()
    {
        // Arrange & Act
        var subscription = new PushSubscriptions
        {
            Endpoint = "https://example.com/push",
            P256dhKey = "BNcRdreALRFXTkOOUHK1EtK2wtaz5Ry4YfYCA_0QTpQtUbVlUls0VJXg7A8u-Ts1XbjhazAkj7I99e8QcYP7DkM",
            AuthKey = "tBHItJI5svbpez7KI4CCXg"
        };

        // Assert
        Assert.Equal("BNcRdreALRFXTkOOUHK1EtK2wtaz5Ry4YfYCA_0QTpQtUbVlUls0VJXg7A8u-Ts1XbjhazAkj7I99e8QcYP7DkM", subscription.P256dhKey);
    }

    [Fact]
    public void RequireAuthKey()
    {
        // Arrange & Act
        var subscription = new PushSubscriptions
        {
            Endpoint = "https://example.com/push",
            P256dhKey = "test-p256dh",
            AuthKey = "tBHItJI5svbpez7KI4CCXg"
        };

        // Assert
        Assert.Equal("tBHItJI5svbpez7KI4CCXg", subscription.AuthKey);
    }

    [Fact]
    public void AllowNullableUserId()
    {
        // Arrange & Act
        var subscription = new PushSubscriptions
        {
            UserId = null,
            Endpoint = "https://example.com/push",
            P256dhKey = "test-p256dh",
            AuthKey = "test-auth"
        };

        // Assert
        Assert.Null(subscription.UserId);
    }

    [Fact]
    public void AllowUserIdToBeSet()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var subscription = new PushSubscriptions
        {
            UserId = userId,
            Endpoint = "https://example.com/push",
            P256dhKey = "test-p256dh",
            AuthKey = "test-auth"
        };

        // Assert
        Assert.Equal(userId, subscription.UserId);
    }

    [Fact]
    public void AllowNullableLastUsedAt()
    {
        // Arrange & Act
        var subscription = new PushSubscriptions
        {
            Endpoint = "https://example.com/push",
            P256dhKey = "test-p256dh",
            AuthKey = "test-auth",
            LastUsedAt = null
        };

        // Assert
        Assert.Null(subscription.LastUsedAt);
    }

    [Fact]
    public void TrackLastUsedAt()
    {
        // Arrange
        var lastUsed = DateTime.Now;

        // Act
        var subscription = new PushSubscriptions
        {
            Endpoint = "https://example.com/push",
            P256dhKey = "test-p256dh",
            AuthKey = "test-auth",
            LastUsedAt = lastUsed
        };

        // Assert
        Assert.Equal(lastUsed, subscription.LastUsedAt);
    }

    [Fact]
    public void InheritFromEntity()
    {
        // Arrange & Act
        var subscription = new PushSubscriptions
        {
            Endpoint = "https://example.com/push",
            P256dhKey = "test-p256dh",
            AuthKey = "test-auth"
        };

        // Assert - Entity properties are accessible
        Assert.NotEqual(default, subscription.Id);
    }
}
