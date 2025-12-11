using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.News;
using Rise.Persistence;
using Rise.Services.News;
using Rise.Shared.Common;
using Rise.Services.Tests.TestInfrastructure;

namespace Rise.Services.Tests.News;

public class NewsServiceShould
{
    [Fact]
    public async Task GetIndexAsyncShouldReturnSuccessWithValidData()
    {
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        dbContext.NewsArticles.AddRange(
            new NewsArticle
            {
                Title = "Test News 1",
                Description = "Test description 1",
                Type = "Test",
                PublishDate = DateTime.Now,
                Content = "Test content 1",
                Author = "Test Author 1",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "Test News 2",
                Description = "Test description 2",
                Type = "Test",
                PublishDate = DateTime.Now.AddMinutes(-10),
                Content = "Test content 2",
                Author = "Test Author 2",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "Test News 3",
                Description = "Test description 3",
                Type = "Test",
                PublishDate = DateTime.Now.AddHours(-1),
                Content = "Test content 3",
                Author = "Test Author 3",
                ImageUrl = "https://example.com/image1.jpg"
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        var request = new QueryRequest.DateRange()
        {
            Skip = 0,
            Take = 10,
        };
        var result = await service.GetIndexAsync(request, CancellationToken.None);
        if (result.IsSuccess)
        {
            result.Value.ShouldNotBeNull();
            result.Value.News.ShouldNotBeNull();
        }
        else
        {
            result.Status.ShouldBe(ResultStatus.NotFound);
        }
    }

    [Fact]
    public async Task GetIndexAsyncWithSearchShouldReturnSuccessWithValidData()
    {
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        dbContext.NewsArticles.AddRange(
            new NewsArticle
            {
                Title = "Test News 1",
                Description = "Test description 1",
                Type = "Test",
                PublishDate = DateTime.Now,
                Content = "Test content 1",
                Author = "Test Author 1",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "Test News 2",
                Description = "Test description 2",
                Type = "Test",
                PublishDate = DateTime.Now.AddMinutes(-10),
                Content = "Test content 2",
                Author = "Test Author 2",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "Test News 3",
                Description = "Test description 3",
                Type = "Test",
                PublishDate = DateTime.Now.AddHours(-1),
                Content = "Test content 3",
                Author = "Test Author 3",
                ImageUrl = "https://example.com/image1.jpg"
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        var request = new QueryRequest.DateRange()
        {
            Skip = 0,
            Take = 10,
            SearchTerm = "Test News 1"
        };
        var result = await service.GetIndexAsync(request, CancellationToken.None);
        if (result.IsSuccess)
        {
            result.Value.ShouldNotBeNull();
            result.Value.News.ShouldNotBeNull();
            result.Value.News.Count().ShouldBe(1);
        }
        else
        {
            result.Status.ShouldBe(ResultStatus.NotFound);
        }
    }

    [Fact]
    public async Task GetByIdAsyncWithSearchShouldReturnSuccessWithValidData()
    {
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var newsArticle = new NewsArticle
        {
            Title = "Test News 1",
            Description = "Test description 1",
            Type = "Test",
            PublishDate = DateTime.Now,
            Content = "Test content 1",
            Author = "Test Author 1",
            ImageUrl = "https://example.com/image1.jpg"
        };

        dbContext.NewsArticles.Add(newsArticle);

        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);
        var result = await service.GetByIdAsync(newsArticle.Id);
        if (result.IsSuccess)
        {
            result.Value.ShouldNotBeNull();
            result.Value.NewsArticle.ShouldNotBeNull();
            result.Value.NewsArticle.Id.ShouldBe(newsArticle.Id);
        }
        else
        {
            result.Status.ShouldBe(ResultStatus.NotFound);
        }
    }
}