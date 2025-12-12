using Rise.Domain.Notifications;

namespace Rise.Domain.Tests.Notifications;

public class NotificationPreferencesShould
{
    [Fact]
    public void BeCreatedWithId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var preferences = new NotificationPreferences(userId);

        // Assert
        Assert.Equal(userId, preferences.Id);
    }

    [Fact]
    public void HaveDefaultValuesSetToTrue()
    {
        // Arrange & Act
        var preferences = new NotificationPreferences(Guid.NewGuid());

        // Assert
        Assert.True(preferences.GradesNotifications);
        Assert.True(preferences.ScheduleNotifications);
        Assert.True(preferences.CampusNotifications);
        Assert.True(preferences.NewsNotifications);
    }

    [Fact]
    public void AllowGradesNotificationsToBeDisabled()
    {
        // Arrange
        var preferences = new NotificationPreferences(Guid.NewGuid());

        // Act
        preferences.GradesNotifications = false;

        // Assert
        Assert.False(preferences.GradesNotifications);
    }

    [Fact]
    public void AllowScheduleNotificationsToBeDisabled()
    {
        // Arrange
        var preferences = new NotificationPreferences(Guid.NewGuid());

        // Act
        preferences.ScheduleNotifications = false;

        // Assert
        Assert.False(preferences.ScheduleNotifications);
    }

    [Fact]
    public void AllowCampusNotificationsToBeDisabled()
    {
        // Arrange
        var preferences = new NotificationPreferences(Guid.NewGuid());

        // Act
        preferences.CampusNotifications = false;

        // Assert
        Assert.False(preferences.CampusNotifications);
    }

    [Fact]
    public void AllowNewsNotificationsToBeDisabled()
    {
        // Arrange
        var preferences = new NotificationPreferences(Guid.NewGuid());

        // Act
        preferences.NewsNotifications = false;

        // Assert
        Assert.False(preferences.NewsNotifications);
    }

    [Fact]
    public void AllowAllNotificationsToBeConfiguredIndependently()
    {
        // Arrange
        var preferences = new NotificationPreferences(Guid.NewGuid());

        // Act
        preferences.GradesNotifications = true;
        preferences.ScheduleNotifications = false;
        preferences.CampusNotifications = true;
        preferences.NewsNotifications = false;

        // Assert
        Assert.True(preferences.GradesNotifications);
        Assert.False(preferences.ScheduleNotifications);
        Assert.True(preferences.CampusNotifications);
        Assert.False(preferences.NewsNotifications);
    }

    [Fact]
    public void InheritFromEntity()
    {
        // Arrange & Act
        var preferences = new NotificationPreferences(Guid.NewGuid());

        // Assert - Entity properties are accessible
        Assert.NotEqual(default, preferences.Id);
    }
}
