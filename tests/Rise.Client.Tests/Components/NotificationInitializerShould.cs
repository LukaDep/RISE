using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using Rise.Client.Components;
using Rise.Shared.Notifications;
using Xunit;

namespace Rise.Client.Tests.Components;

public class NotificationInitializerShould : TestContext
{
    private readonly IJSRuntime _jsRuntime;
    private readonly INotificationPreferencesService _notificationService;
    private readonly AuthenticationStateProvider _authStateProvider;

    public NotificationInitializerShould()
    {
        _jsRuntime = Substitute.For<IJSRuntime>();
        _notificationService = Substitute.For<INotificationPreferencesService>();
        _authStateProvider = new FakeAuthStateProvider(true);

        Services.AddSingleton(_jsRuntime);
        Services.AddSingleton(_notificationService);
        Services.AddSingleton<AuthenticationStateProvider>(_authStateProvider);
        Services.AddLocalization();
    }

    #region Rendering

    [Fact]
    public async Task NotShow_WhenPermissionAlreadyGranted()
    {
        // Arrange
        _jsRuntime.InvokeAsync<string>("eval", Arg.Any<object[]>())
            .Returns(new ValueTask<string>("granted"));

        // Act
        var cut = RenderComponent<NotificationInitializer>();
        await cut.InvokeAsync(() => { }); // Wait for async init

        // Assert - prompt should not be shown
        Assert.False(cut.Instance.Show);
    }

    [Fact]
    public async Task NotShow_WhenPermissionAlreadyDenied()
    {
        // Arrange
        _jsRuntime.InvokeAsync<string>("eval", Arg.Any<object[]>())
            .Returns(new ValueTask<string>("denied"));

        // Act
        var cut = RenderComponent<NotificationInitializer>();
        await cut.InvokeAsync(() => { }); // Wait for async init

        // Assert - prompt should not be shown
        Assert.False(cut.Instance.Show);
    }

    [Fact]
    public async Task Show_WhenPermissionIsDefault_AndNoExistingSubscription()
    {
        // Arrange
        _jsRuntime.InvokeAsync<string>("eval", Arg.Any<object[]>())
            .Returns(new ValueTask<string>("default"));
        _jsRuntime.InvokeAsync<bool>("checkExistingSubscription", Arg.Any<object[]>())
            .Returns(new ValueTask<bool>(false));

        // Act
        var cut = RenderComponent<NotificationInitializer>();
        await Task.Delay(100); // Wait for async init

        // Assert - prompt should be shown
        Assert.True(cut.Instance.Show);
    }

    [Fact]
    public async Task NotShow_WhenPermissionIsDefault_ButSubscriptionExists()
    {
        // Arrange
        _jsRuntime.InvokeAsync<string>("eval", Arg.Any<object[]>())
            .Returns(new ValueTask<string>("default"));
        _jsRuntime.InvokeAsync<bool>("checkExistingSubscription", Arg.Any<object[]>())
            .Returns(new ValueTask<bool>(true));

        // Act
        var cut = RenderComponent<NotificationInitializer>();
        await Task.Delay(100); // Wait for async init

        // Assert - prompt should not be shown because subscription exists
        Assert.False(cut.Instance.Show);
    }

    [Fact]
    public async Task ShowAllowAndDenyButtons_WhenVisible()
    {
        // Arrange
        _jsRuntime.InvokeAsync<string>("eval", Arg.Any<object[]>())
            .Returns(new ValueTask<string>("default"));
        _jsRuntime.InvokeAsync<bool>("checkExistingSubscription", Arg.Any<object[]>())
            .Returns(new ValueTask<bool>(false));

        // Act
        var cut = RenderComponent<NotificationInitializer>();
        await Task.Delay(100); // Wait for async init

        // Assert
        var buttons = cut.FindAll("button");
        Assert.Equal(2, buttons.Count);
    }

    #endregion

    #region Allow Button

    [Fact]
    public async Task CallSubscribe_WhenAllowClicked()
    {
        // Arrange
        _jsRuntime.InvokeAsync<string>("eval", Arg.Any<object[]>())
            .Returns(new ValueTask<string>("default"));
        _jsRuntime.InvokeAsync<bool>("checkExistingSubscription", Arg.Any<object[]>())
            .Returns(new ValueTask<bool>(false));

        var subscriptionData = new PushSubscriptionRequest.Create
        {
            Endpoint = "https://fcm.googleapis.com/test",
            Keys = new PushSubscriptionRequest.Keys { P256dh = "key", Auth = "auth" }
        };

        _jsRuntime.InvokeAsync<PushSubscriptionRequest.Create>("subscribeUser", Arg.Any<object[]>())
            .Returns(new ValueTask<PushSubscriptionRequest.Create>(subscriptionData));

        _notificationService.Subscribe(Arg.Any<PushSubscriptionRequest.Create>())
            .Returns(Ardalis.Result.Result.Success());

        var cut = RenderComponent<NotificationInitializer>();
        await Task.Delay(100); // Wait for async init

        // Act
        var allowButton = cut.FindAll("button")[1]; // Allow is second button
        await cut.InvokeAsync(() => allowButton.Click());

        // Assert
        await _notificationService.Received(1).Subscribe(Arg.Any<PushSubscriptionRequest.Create>());
    }

    [Fact]
    public async Task HidePrompt_AfterAllow()
    {
        // Arrange
        _jsRuntime.InvokeAsync<string>("eval", Arg.Any<object[]>())
            .Returns(new ValueTask<string>("default"));
        _jsRuntime.InvokeAsync<bool>("checkExistingSubscription", Arg.Any<object[]>())
            .Returns(new ValueTask<bool>(false));

        var subscriptionData = new PushSubscriptionRequest.Create
        {
            Endpoint = "https://fcm.googleapis.com/test",
            Keys = new PushSubscriptionRequest.Keys { P256dh = "key", Auth = "auth" }
        };

        _jsRuntime.InvokeAsync<PushSubscriptionRequest.Create>("subscribeUser", Arg.Any<object[]>())
            .Returns(new ValueTask<PushSubscriptionRequest.Create>(subscriptionData));

        _notificationService.Subscribe(Arg.Any<PushSubscriptionRequest.Create>())
            .Returns(Ardalis.Result.Result.Success());

        var cut = RenderComponent<NotificationInitializer>();
        await Task.Delay(100); // Wait for async init

        // Act
        var allowButton = cut.FindAll("button")[1];
        await cut.InvokeAsync(() => allowButton.Click());

        // Assert - prompt should be hidden
        Assert.False(cut.Instance.Show);
    }

    #endregion

    #region Deny Button

    [Fact]
    public async Task HidePrompt_AfterDeny()
    {
        // Arrange
        _jsRuntime.InvokeAsync<string>("eval", Arg.Any<object[]>())
            .Returns(new ValueTask<string>("default"));
        _jsRuntime.InvokeAsync<bool>("checkExistingSubscription", Arg.Any<object[]>())
            .Returns(new ValueTask<bool>(false));

        var cut = RenderComponent<NotificationInitializer>();
        await Task.Delay(100); // Wait for async init

        // Act
        var denyButton = cut.FindAll("button")[0]; // Deny is first button
        await cut.InvokeAsync(() => denyButton.Click());

        // Assert - prompt should be hidden
        Assert.False(cut.Instance.Show);
    }

    #endregion

    #region VapIdPublicKey Parameter

    [Fact]
    public void HaveDefaultVapIdPublicKey()
    {
        // Arrange & Act
        var cut = RenderComponent<NotificationInitializer>();

        // Assert
        Assert.NotNull(cut.Instance.VapIdPublicKey);
        Assert.NotEmpty(cut.Instance.VapIdPublicKey);
    }

    [Fact]
    public void AcceptCustomVapIdPublicKey()
    {
        // Arrange
        var customKey = "custom-vapid-key";

        // Act
        var cut = RenderComponent<NotificationInitializer>(parameters => parameters
            .Add(p => p.VapIdPublicKey, customKey));

        // Assert
        Assert.Equal(customKey, cut.Instance.VapIdPublicKey);
    }

    #endregion

    #region Error Handling

    // Note: NotificationInitializer does not catch JS exceptions - they propagate to the renderer.
    // This is expected behavior for first-render operations where we cannot retry.

    #endregion

    #region FakeAuthStateProvider

    private class FakeAuthStateProvider : AuthenticationStateProvider
    {
        private readonly bool _isAuthenticated;

        public FakeAuthStateProvider(bool isAuthenticated)
        {
            _isAuthenticated = isAuthenticated;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = _isAuthenticated
                ? new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "testuser")
                }, "test")
                : new System.Security.Claims.ClaimsIdentity();

            var user = new System.Security.Claims.ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
    }

    #endregion
}
