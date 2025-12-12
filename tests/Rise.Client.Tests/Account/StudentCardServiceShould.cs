using System.Net;
using System.Text;
using System.Text.Json;
using Ardalis.Result;
using Rise.Shared.Identity.Accounts;
using Rise.Shared.StudentCards;

namespace Rise.Client.Account;

public class StudentCardServiceShould
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
    public async Task GetByUserIdAsync_WithValidStudentCard_ShouldReturnSuccess()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "123456789",
            FirstName = "John",
            LastName = "Doe",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = "https://example.com/profile.jpg",
            IsValid = true
        };

        var accountInfo = new AccountResponse.Info
        {
            Email = "john@example.com",
            IsEmailConfirmed = true,
            Claims = new Dictionary<string, string>(),
            StudentCard = studentCard
        };

        var responseData = Result.Success(accountInfo);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new StudentCardService(httpClient);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("123456789", result.Value.PersonalNumber);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNoStudentCard_ShouldReturnNotFound()
    {
        // Arrange
        var accountInfo = new AccountResponse.Info
        {
            Email = "test@example.com",
            IsEmailConfirmed = true,
            Claims = new Dictionary<string, string>(),
            StudentCard = null
        };

        var responseData = Result.Success(accountInfo);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new StudentCardService(httpClient);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNullResponse_ShouldReturnNotFound()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new StudentCardService(httpClient);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithValidCard_ShouldReturnAllProperties()
    {
        // Arrange
        var expirationDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "987654321",
            FirstName = "Jane",
            LastName = "Smith",
            ExpirationDate = expirationDate,
            ProfilePicture = "https://example.com/jane.png",
            IsValid = true
        };

        var accountInfo = new AccountResponse.Info
        {
            Email = "jane@example.com",
            IsEmailConfirmed = true,
            Claims = new Dictionary<string, string>(),
            StudentCard = studentCard
        };

        var responseData = Result.Success(accountInfo);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new StudentCardService(httpClient);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("987654321", result.Value.PersonalNumber);
        Assert.Equal("Jane", result.Value.FirstName);
        Assert.Equal("Smith", result.Value.LastName);
        Assert.Equal(expirationDate, result.Value.ExpirationDate);
        Assert.Equal("https://example.com/jane.png", result.Value.ProfilePicture);
        Assert.True(result.Value.IsValid);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithExpiredCard_ShouldReturnIsValidFalse()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "111222333",
            FirstName = "Expired",
            LastName = "Card",
            ExpirationDate = DateTime.UtcNow.AddDays(-30),
            ProfilePicture = null,
            IsValid = false
        };

        var accountInfo = new AccountResponse.Info
        {
            Email = "expired@example.com",
            IsEmailConfirmed = true,
            Claims = new Dictionary<string, string>(),
            StudentCard = studentCard
        };

        var responseData = Result.Success(accountInfo);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new StudentCardService(httpClient);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsValid);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNullProfilePicture_ShouldReturnNull()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "444555666",
            FirstName = "No",
            LastName = "Picture",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = null,
            IsValid = true
        };

        var accountInfo = new AccountResponse.Info
        {
            Email = "nopic@example.com",
            IsEmailConfirmed = true,
            Claims = new Dictionary<string, string>(),
            StudentCard = studentCard
        };

        var responseData = Result.Success(accountInfo);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new StudentCardService(httpClient);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.ProfilePicture);
    }
}
