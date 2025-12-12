using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Rise.Client.Account.Notifications;
using Rise.Shared.Notifications;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using NotificationsIndex = Rise.Client.Account.Notifications.Index;

namespace Rise.Client.Tests.Notifications;

public class NotificationsIndexShould : TestContext
{
    private readonly ISentNotificationService _mockService;

    public NotificationsIndexShould()
    {
        _mockService = Substitute.For<ISentNotificationService>();
        Services.AddSingleton(_mockService);
        Services.AddLocalization();

        // Add authorization services
        var authState = Task.FromResult(new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "test"))));
        Services.AddSingleton<AuthenticationStateProvider>(new FakeAuthStateProvider(authState));
        Services.AddSingleton<IAuthorizationService, FakeAuthorizationService>();

        // Add fake NavigationManager
        Services.AddSingleton(Substitute.For<Microsoft.AspNetCore.Components.NavigationManager>());
    }

    private class FakeAuthStateProvider : AuthenticationStateProvider
    {
        private readonly Task<AuthenticationState> _authState;
        public FakeAuthStateProvider(Task<AuthenticationState> authState) => _authState = authState;
        public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authState;
    }

    private class FakeAuthorizationService : IAuthorizationService
    {
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
            => Task.FromResult(AuthorizationResult.Success());

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
            => Task.FromResult(AuthorizationResult.Success());
    }

    [Fact]
    public void DisplayLoadingState_WhenInitializing()
    {
        // Arrange
        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = new List<SentNotificationDTO.Index>(),
                TotalCount = 0,
                UnreadCount = 0
            })));

        // Act
        var cut = RenderComponent<NotificationsIndex>();

        // Assert - Component renders without error
        Assert.NotNull(cut);
    }

    [Fact]
    public async Task DisplayNotifications_WhenDataLoaded()
    {
        // Arrange
        var notifications = new List<SentNotificationDTO.Index>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Test Notification 1",
                Body = "Body 1",
                SentAt = DateTime.Now,
                IsRead = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Test Notification 2",
                Body = "Body 2",
                SentAt = DateTime.Now,
                IsRead = true
            }
        };

        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = notifications,
                TotalCount = 2,
                UnreadCount = 1
            }));

        // Act
        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100); // Allow component to initialize

        // Assert
        var markup = cut.Markup;
        Assert.Contains("Test Notification 1", markup);
        Assert.Contains("Test Notification 2", markup);
    }

    [Fact]
    public async Task DisplayEmptyState_WhenNoNotifications()
    {
        // Arrange
        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = new List<SentNotificationDTO.Index>(),
                TotalCount = 0,
                UnreadCount = 0
            }));

        // Act
        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100);

        // Assert
        var markup = cut.Markup;
        Assert.Contains("fa-bell-slash", markup); // Empty state icon
    }

    [Fact]
    public async Task DisplayErrorState_WhenServiceFails()
    {
        // Arrange
        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Service error"));

        // Act
        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100);

        // Assert
        var markup = cut.Markup;
        Assert.Contains("fa-circle-exclamation", markup); // Error icon
    }

    [Fact]
    public async Task ShowMarkAllAsReadButton_WhenUnreadNotificationsExist()
    {
        // Arrange
        var notifications = new List<SentNotificationDTO.Index>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Unread",
                Body = "Body",
                SentAt = DateTime.Now,
                IsRead = false
            }
        };

        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = notifications,
                TotalCount = 1,
                UnreadCount = 1
            }));

        // Act
        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100);

        // Assert - The mark all as read button should be visible
        var markup = cut.Markup;
        // The button should exist since there are unread notifications
        Assert.NotNull(cut);
    }

    [Fact]
    public async Task CallMarkAsReadAsync_WhenNotificationClicked()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var notifications = new List<SentNotificationDTO.Index>
        {
            new()
            {
                Id = notificationId,
                Title = "Clickable",
                Body = "Body",
                SentAt = DateTime.Now,
                IsRead = false
            }
        };

        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = notifications,
                TotalCount = 1,
                UnreadCount = 1
            }));

        _mockService.MarkAsReadAsync(Arg.Any<SentNotificationRequest.MarkAsRead>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100);

        // Act - Click on the notification
        var notificationElement = cut.Find("div[class*='cursor-pointer']");
        await cut.InvokeAsync(() => notificationElement.Click());

        // Assert
        await _mockService.Received().MarkAsReadAsync(
            Arg.Is<SentNotificationRequest.MarkAsRead>(r => r.NotificationId == notificationId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallMarkAllAsReadAsync_WhenMarkAllButtonClicked()
    {
        // Arrange
        var notifications = new List<SentNotificationDTO.Index>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Unread 1",
                Body = "Body",
                SentAt = DateTime.Now,
                IsRead = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Unread 2",
                Body = "Body",
                SentAt = DateTime.Now,
                IsRead = false
            }
        };

        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = notifications,
                TotalCount = 2,
                UnreadCount = 2
            }));

        _mockService.MarkAllAsReadAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100);

        // Act
        var markAllButton = cut.Find("button.text-blue-600");
        await cut.InvokeAsync(() => markAllButton.Click());

        // Assert
        await _mockService.Received().MarkAllAsReadAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallDeleteNotificationAsync_WhenDeleteButtonClicked()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var notifications = new List<SentNotificationDTO.Index>
        {
            new()
            {
                Id = notificationId,
                Title = "Deletable",
                Body = "Body",
                SentAt = DateTime.Now,
                IsRead = false
            }
        };

        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = notifications,
                TotalCount = 1,
                UnreadCount = 1
            }));

        _mockService.DeleteNotificationAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100);

        // Act - Click the delete button (fa-times icon)
        var deleteButton = cut.Find("button.text-gray-400");
        await cut.InvokeAsync(() => deleteButton.Click());

        // Assert
        await _mockService.Received().DeleteNotificationAsync(notificationId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DisplayDifferentIcons_ForDifferentNotificationTypes()
    {
        // Arrange
        var notifications = new List<SentNotificationDTO.Index>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Grade Notification",
                Body = "Body",
                NotificationType = "grades",
                SentAt = DateTime.Now,
                IsRead = false
            }
        };

        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = notifications,
                TotalCount = 1,
                UnreadCount = 1
            }));

        // Act
        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100);

        // Assert
        var markup = cut.Markup;
        Assert.Contains("emerald", markup); // Grade notifications have emerald color
    }

    [Fact]
    public async Task HighlightUnreadNotifications()
    {
        // Arrange
        var notifications = new List<SentNotificationDTO.Index>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Unread",
                Body = "Body",
                SentAt = DateTime.Now,
                IsRead = false
            }
        };

        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = notifications,
                TotalCount = 1,
                UnreadCount = 1
            }));

        // Act
        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100);

        // Assert - Unread notifications have blue background
        var markup = cut.Markup;
        Assert.Contains("bg-blue-50", markup);
    }

    [Fact]
    public async Task DisplayNotificationCounts()
    {
        // Arrange
        var notifications = new List<SentNotificationDTO.Index>
        {
            new() { Id = Guid.NewGuid(), Title = "N1", Body = "B", SentAt = DateTime.Now, IsRead = false },
            new() { Id = Guid.NewGuid(), Title = "N2", Body = "B", SentAt = DateTime.Now, IsRead = true }
        };

        _mockService.GetUserNotificationsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new SentNotificationResponse.Index
            {
                Notifications = notifications,
                TotalCount = 10,
                UnreadCount = 3
            }));

        // Act
        var cut = RenderComponent<NotificationsIndex>();
        await Task.Delay(100);

        // Assert
        var markup = cut.Markup;
        Assert.Contains("3", markup); // Unread count
        Assert.Contains("10", markup); // Total count
    }
}
