using System.Net;
using System.Text;
using System.Text.Json;
using Ardalis.Result;
using Microsoft.JSInterop;
using NSubstitute;
using Rise.Client.Notifications;
using Rise.Shared.Notifications;

namespace Rise.Client.Tests.Notifications;

public class NotificationPreferencesClientServiceShould
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        public TestHttpMessageHandler(HttpResponseMessage response)
        {
            _handler = _ => response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }

    private static HttpClient CreateMockHttpClient(HttpResponseMessage response)
    {
        var handler = new TestHttpMessageHandler(response);
        return new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
    }

    private static HttpClient CreateMockHttpClient(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        var msgHandler = new TestHttpMessageHandler(handler);
        return new HttpClient(msgHandler) { BaseAddress = new Uri("http://localhost/") };
    }

    #region GetUserPreferencesByIdAsync

    [Fact]
    public async Task GetUserPreferencesByIdAsync_WithSuccessResponse_ShouldReturnPreferences()
    {
        // Arrange
        var prefs = new NotificationPreferencesDTO.Index
        {
            UserId = Guid.NewGuid(),
            GradesNotifications = true,
            ScheduleNotifications = false,
            CampusNotifications = true,
            NewsNotifications = false,
            IsEnabled = true
        };

        var responseData = Result.Success(new NotificationPreferencesResponse.Index
        {
            NotificationPreference = prefs
        });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();
        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.GetUserPreferencesByIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value?.NotificationPreference);
        Assert.True(result.Value.NotificationPreference.GradesNotifications);
        Assert.False(result.Value.NotificationPreference.ScheduleNotifications);
    }

    #endregion

    #region EditAsync

    [Fact]
    public async Task EditAsync_WithSuccessResponse_ShouldReturnSuccess()
    {
        // Arrange
        var responseData = Result.Success();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();
        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        var request = new NotificationPreferencesRequest.Edit
        {
            GradesNotifications = true,
            ScheduleNotifications = false,
            CampusNotifications = true,
            NewsNotifications = false
        };

        // Act
        var result = await service.EditAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task EditAsync_WithErrorResponse_ShouldReturnError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();
        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.EditAsync(new NotificationPreferencesRequest.Edit(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Fout bij het opslaan van notificatie-instellingen", result.Errors.First());
    }

    [Fact]
    public async Task EditAsync_WithNullResponseContent_ShouldReturnSuccess()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();
        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.EditAsync(new NotificationPreferencesRequest.Edit(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    #endregion

    #region Subscribe

    [Fact]
    public async Task Subscribe_WithSuccessfulJsAndHttpResponse_ShouldReturnSuccess()
    {
        // Arrange
        var responseData = Result.Success();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();

        var subscriptionData = new PushSubscriptionRequest.Create
        {
            Endpoint = "https://fcm.googleapis.com/test",
            Keys = new PushSubscriptionRequest.Keys { P256dh = "test-key", Auth = "test-auth" }
        };

        jsRuntime.InvokeAsync<PushSubscriptionRequest.Create>("subscribeUser", Arg.Any<object[]>())
            .Returns(new ValueTask<PushSubscriptionRequest.Create>(subscriptionData));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.Subscribe(new PushSubscriptionRequest.Create
        {
            Endpoint = string.Empty,
            Keys = new PushSubscriptionRequest.Keys { P256dh = string.Empty, Auth = string.Empty }
        });

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Subscribe_WithJSException_ShouldReturnError()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var jsRuntime = Substitute.For<IJSRuntime>();

        jsRuntime.When(x => x.InvokeAsync<PushSubscriptionRequest.Create>("subscribeUser", Arg.Any<object[]>()))
            .Do(_ => throw new JSException("Permission denied"));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.Subscribe(new PushSubscriptionRequest.Create
        {
            Endpoint = string.Empty,
            Keys = new PushSubscriptionRequest.Keys { P256dh = string.Empty, Auth = string.Empty }
        });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Push-meldingen worden niet ondersteund of zijn geblokkeerd", result.Errors.First());
    }

    [Fact]
    public async Task Subscribe_WithEmptyJSExceptionMessage_ShouldReturnDefaultError()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var jsRuntime = Substitute.For<IJSRuntime>();

        jsRuntime.When(x => x.InvokeAsync<PushSubscriptionRequest.Create>("subscribeUser", Arg.Any<object[]>()))
            .Do(_ => throw new JSException(string.Empty));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.Subscribe(new PushSubscriptionRequest.Create
        {
            Endpoint = string.Empty,
            Keys = new PushSubscriptionRequest.Keys { P256dh = string.Empty, Auth = string.Empty }
        });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Onbekende JavaScript fout", result.Errors.First());
    }

    [Fact]
    public async Task Subscribe_WithHttpError_ShouldReturnError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();

        var subscriptionData = new PushSubscriptionRequest.Create
        {
            Endpoint = "https://fcm.googleapis.com/test",
            Keys = new PushSubscriptionRequest.Keys { P256dh = "test-key", Auth = "test-auth" }
        };

        jsRuntime.InvokeAsync<PushSubscriptionRequest.Create>("subscribeUser", Arg.Any<object[]>())
            .Returns(new ValueTask<PushSubscriptionRequest.Create>(subscriptionData));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.Subscribe(new PushSubscriptionRequest.Create
        {
            Endpoint = string.Empty,
            Keys = new PushSubscriptionRequest.Keys { P256dh = string.Empty, Auth = string.Empty }
        });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Fout bij het aanmelden voor push-meldingen", result.Errors.First());
    }

    [Fact]
    public async Task Subscribe_WithUnexpectedException_ShouldReturnError()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var jsRuntime = Substitute.For<IJSRuntime>();

        jsRuntime.When(x => x.InvokeAsync<PushSubscriptionRequest.Create>("subscribeUser", Arg.Any<object[]>()))
            .Do(_ => throw new InvalidOperationException("Unexpected error"));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.Subscribe(new PushSubscriptionRequest.Create
        {
            Endpoint = string.Empty,
            Keys = new PushSubscriptionRequest.Keys { P256dh = string.Empty, Auth = string.Empty }
        });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Unexpected error", result.Errors.First());
    }

    #endregion

    #region Unsubscribe

    [Fact]
    public async Task Unsubscribe_WithSuccessResponse_ShouldReturnSuccess()
    {
        // Arrange
        var responseData = Result.Success();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();
        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.Unsubscribe();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Unsubscribe_WithErrorResponse_ShouldReturnError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();
        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.Unsubscribe();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Fout bij het afmelden voor push-meldingen", result.Errors.First());
    }

    [Fact]
    public async Task Unsubscribe_WithNullResponseContent_ShouldReturnSuccess()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();
        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.Unsubscribe();

        // Assert
        Assert.True(result.IsSuccess);
    }

    #endregion

    #region SyncSubscriptionAsync

    [Fact]
    public async Task SyncSubscriptionAsync_WithNoExistingSubscription_ShouldReturnSuccess()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var jsRuntime = Substitute.For<IJSRuntime>();

        jsRuntime.InvokeAsync<PushSubscriptionRequest.Create?>("getExistingSubscription", Arg.Any<object[]>())
            .Returns(new ValueTask<PushSubscriptionRequest.Create?>((PushSubscriptionRequest.Create?)null));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.SyncSubscriptionAsync();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SyncSubscriptionAsync_WithExistingSubscription_ShouldSyncWithServer()
    {
        // Arrange
        var responseData = Result.Success();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();

        var subscriptionData = new PushSubscriptionRequest.Create
        {
            Endpoint = "https://fcm.googleapis.com/test",
            Keys = new PushSubscriptionRequest.Keys { P256dh = "test-key", Auth = "test-auth" }
        };

        jsRuntime.InvokeAsync<PushSubscriptionRequest.Create?>("getExistingSubscription", Arg.Any<object[]>())
            .Returns(new ValueTask<PushSubscriptionRequest.Create?>(subscriptionData));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.SyncSubscriptionAsync();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SyncSubscriptionAsync_WithHttpError_ShouldReturnError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        var httpClient = CreateMockHttpClient(response);
        var jsRuntime = Substitute.For<IJSRuntime>();

        var subscriptionData = new PushSubscriptionRequest.Create
        {
            Endpoint = "https://fcm.googleapis.com/test",
            Keys = new PushSubscriptionRequest.Keys { P256dh = "test-key", Auth = "test-auth" }
        };

        jsRuntime.InvokeAsync<PushSubscriptionRequest.Create?>("getExistingSubscription", Arg.Any<object[]>())
            .Returns(new ValueTask<PushSubscriptionRequest.Create?>(subscriptionData));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.SyncSubscriptionAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task SyncSubscriptionAsync_WithJSException_ShouldReturnSuccess()
    {
        // Arrange - sync errors should not be fatal
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var jsRuntime = Substitute.For<IJSRuntime>();

        jsRuntime.When(x => x.InvokeAsync<PushSubscriptionRequest.Create?>("getExistingSubscription", Arg.Any<object[]>()))
            .Do(_ => throw new JSException("JS error"));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.SyncSubscriptionAsync();

        // Assert - Should succeed despite JS error (sync failures are not fatal)
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SyncSubscriptionAsync_WithUnexpectedException_ShouldReturnSuccess()
    {
        // Arrange - sync errors should not be fatal
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var jsRuntime = Substitute.For<IJSRuntime>();

        jsRuntime.When(x => x.InvokeAsync<PushSubscriptionRequest.Create?>("getExistingSubscription", Arg.Any<object[]>()))
            .Do(_ => throw new InvalidOperationException("Unexpected error"));

        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act
        var result = await service.SyncSubscriptionAsync();

        // Assert - Should succeed despite exception (sync failures are not fatal)
        Assert.True(result.IsSuccess);
    }

    #endregion

    #region SendTestToUser

    [Fact]
    public async Task SendTestToUser_ShouldThrowNotImplementedException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var jsRuntime = Substitute.For<IJSRuntime>();
        var service = new NotificationPreferencesClientService(httpClient, jsRuntime);

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            service.SendTestToUser(new Push.Send { title = "Test", body = "Body" }));
    }

    #endregion
}
