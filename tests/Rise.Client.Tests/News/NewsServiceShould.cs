using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Ardalis.Result;
using NSubstitute;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public class NewsServiceShould
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
    public async Task GetIndexAsync_ShouldReturnNewsItems()
    {
        // Arrange
        var expectedNews = new List<NewsDto.Index>
        {
            new() { Id = Guid.NewGuid(), Title = "Test News 1", Description = "Desc 1", Type = "Type1", PublishDate = DateTime.UtcNow, Content = "Content 1", Author = "Author 1", ImageUrl = "http://example.com/1.jpg" },
            new() { Id = Guid.NewGuid(), Title = "Test News 2", Description = "Desc 2", Type = "Type2", PublishDate = DateTime.UtcNow, Content = "Content 2", Author = "Author 2", ImageUrl = "http://example.com/2.jpg" }
        };
        var responseData = Result.Success(new NewsResponse.Index { News = expectedNews, TotalCount = 2 });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new NewsService(httpClient);

        var request = new QueryRequest.DateRange { Skip = 0, Take = 10 };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.News);
        Assert.Equal(2, result.Value.News.Count());
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnEmptyListWhenNoNews()
    {
        // Arrange
        var responseData = Result.Success(new NewsResponse.Index { News = new List<NewsDto.Index>(), TotalCount = 0 });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new NewsService(httpClient);

        var request = new QueryRequest.DateRange { Skip = 0, Take = 10, SearchTerm = "nonexistent" };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.News);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNewsArticle()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var expectedNews = new NewsDto.Index
        {
            Id = newsId,
            Title = "Test News",
            Description = "Test Description",
            Type = "TestType",
            PublishDate = DateTime.UtcNow,
            Content = "Test Content",
            Author = "Test Author",
            ImageUrl = "http://example.com/image.jpg"
        };
        var responseData = Result.Success(new NewsResponse.Get { NewsArticle = expectedNews });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new NewsService(httpClient);

        // Act
        var result = await service.GetByIdAsync(newsId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.NewsArticle);
        Assert.Equal(newsId, result.Value.NewsArticle.Id);
        Assert.Equal("Test News", result.Value.NewsArticle.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var responseData = Result<NewsResponse.Get>.NotFound($"News item with id {newsId} not found.");

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new NewsService(httpClient);

        // Act
        var result = await service.GetByIdAsync(newsId);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetIndexAsync_WithPagination_ShouldReturnCorrectCount()
    {
        // Arrange
        var expectedNews = new List<NewsDto.Index>
        {
            new() { Id = Guid.NewGuid(), Title = "News 1", Description = "Desc", Type = "Type", PublishDate = DateTime.UtcNow, Content = "Content", Author = "Author", ImageUrl = "http://example.com/1.jpg" },
            new() { Id = Guid.NewGuid(), Title = "News 2", Description = "Desc", Type = "Type", PublishDate = DateTime.UtcNow, Content = "Content", Author = "Author", ImageUrl = "http://example.com/2.jpg" }
        };
        var responseData = Result.Success(new NewsResponse.Index { News = expectedNews, TotalCount = 100 });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new NewsService(httpClient);

        var request = new QueryRequest.DateRange { Skip = 20, Take = 10 };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsAllNewsProperties()
    {
        // Arrange
        var publishDate = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var expectedNews = new List<NewsDto.Index>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Full News",
                Description = "Full Description",
                Type = "Announcement",
                PublishDate = publishDate,
                Content = "Full Content",
                Author = "Full Author",
                ImageUrl = "http://example.com/full.jpg"
            }
        };
        var responseData = Result.Success(new NewsResponse.Index { News = expectedNews, TotalCount = 1 });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseData, JsonOptions), Encoding.UTF8, "application/json")
        };

        var httpClient = CreateMockHttpClient(response);
        var service = new NewsService(httpClient);

        var request = new QueryRequest.DateRange { Skip = 0, Take = 10 };

        // Act
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        var newsItem = result.Value.News.First();
        Assert.Equal("Full News", newsItem.Title);
        Assert.Equal("Full Description", newsItem.Description);
        Assert.Equal("Announcement", newsItem.Type);
        Assert.Equal("Full Content", newsItem.Content);
        Assert.Equal("Full Author", newsItem.Author);
        Assert.Equal("http://example.com/full.jpg", newsItem.ImageUrl);
    }
}
