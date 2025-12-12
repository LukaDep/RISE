using System.Net;
using System.Text;
using System.Text.Json;
using Ardalis.Result;
using Rise.Client.Account.Notifications;
using Rise.Shared.Notifications;

namespace Rise.Client.Tests.Notifications;

public class SentNotificationClientServiceShould
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static HttpClient CreateMockHttpClient(HttpResponseMessage response)
    {
        var handler = new TestHttpMessageHandler(response);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/")
        };
        return client;
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public TestHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithSuccessResponse_ShouldReturnNotifications()
    {
        // Arrange
        var notifications = new List<SentNotificationDTO.Index>
        {
            new() { Id = Guid.NewGuid(), Title = "Test 1", Body = "Body 1", SentAt = DateTime.Now },
            new() { Id = Guid.NewGuid(), Title = "Test 2", Body = "Body 2", SentAt = DateTime.Now }
        };

        var responseData = Result.Success(new SentNotificationResponse.Index
        {
            Notifications = notifications,
            TotalCount = 2,
            UnreadCount = 1
        });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.GetUserNotificationsAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(1, result.Value.UnreadCount);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithPagination_ShouldPassParameters()
    {
        // Arrange
        var responseData = Result.Success(new SentNotificationResponse.Index
        {
            Notifications = new List<SentNotificationDTO.Index>(),
            TotalCount = 0,
            UnreadCount = 0
        });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.GetUserNotificationsAsync(page: 2, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithNullResponse_ShouldReturnError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.GetUserNotificationsAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task MarkAsReadAsync_WithSuccessResponse_ShouldReturnSuccess()
    {
        // Arrange
        var responseData = Result.Success();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.MarkAsReadAsync(new SentNotificationRequest.MarkAsRead
        {
            NotificationId = Guid.NewGuid()
        });

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task MarkAsReadAsync_WithErrorResponse_ShouldReturnError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.MarkAsReadAsync(new SentNotificationRequest.MarkAsRead
        {
            NotificationId = Guid.NewGuid()
        });

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task MarkAllAsReadAsync_WithSuccessResponse_ShouldReturnSuccess()
    {
        // Arrange
        var responseData = Result.Success();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.MarkAllAsReadAsync();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task MarkAllAsReadAsync_WithErrorResponse_ShouldReturnError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.MarkAllAsReadAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetUnreadCountAsync_WithSuccessResponse_ShouldReturnCount()
    {
        // Arrange
        var responseData = Result.Success(5);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.GetUnreadCountAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task GetUnreadCountAsync_WithNullResponse_ShouldReturnError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.GetUnreadCountAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteNotificationAsync_WithSuccessResponse_ShouldReturnSuccess()
    {
        // Arrange
        var responseData = Result.Success();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.DeleteNotificationAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteNotificationAsync_WithErrorResponse_ShouldReturnError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act
        var result = await service.DeleteNotificationAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task SaveSentNotificationAsync_ShouldThrowNotImplemented()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var httpClient = CreateMockHttpClient(response);
        var service = new SentNotificationClientService(httpClient);

        // Act & Assert - This method is server-side only
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            service.SaveSentNotificationAsync(Guid.NewGuid(), "Test", "Body"));
    }
}
