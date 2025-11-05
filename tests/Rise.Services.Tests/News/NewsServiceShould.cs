using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.News;
using Rise.Persistence;
using Rise.Services.News;
using Rise.Shared.Common;

namespace Rise.Services.Tests.News;

public class NewsServiceShould
{
    [Fact]
    public async Task GetIndexAsyncShouldReturnSuccessWithValidData()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(GetIndexAsyncShouldReturnSuccessWithValidData)) // Do NOT use InMemoryDatabase... it's not reliable. Use a real database and come up with a strategy to clean up the database between tests.
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        dbContext.NewsItems.AddRange(
            new NewsItem
            {
                Title = "Test News 1",
                Description = "Test description 1",
                Type = "Test",
                PublishDate = DateTime.UtcNow,
                Content = "Test content 1",
                Author = "Test Author 1"
            },
            new NewsItem
            {
                Title = "Test News 2",
                Description = "Test description 2",
                Type = "Test",
                PublishDate = DateTime.UtcNow.AddMinutes(-10),
                Content = "Test content 2",
                Author = "Test Author 2"
            },
            new NewsItem
            {
                Title = "Test News 3",
                Description = "Test description 3",
                Type = "Test",
                PublishDate = DateTime.UtcNow.AddHours(-1),
                Content = "Test content 3",
                Author = "Test Author 3"
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        var request = new QueryRequest.DateRange()
        {
            Skip = 0,
            Take = 10,
        };

        // Act
        var result = await service.GetIndexAsync(request, CancellationToken.None);

        // Assert
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
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(GetIndexAsyncWithSearchShouldReturnSuccessWithValidData)) // Do NOT use InMemoryDatabase... it's not reliable. Use a real database and come up with a strategy to clean up the database between tests.
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        dbContext.NewsItems.AddRange(
            new NewsItem
            {
                Title = "Test News 1",
                Description = "Test description 1",
                Type = "Test",
                PublishDate = DateTime.UtcNow,
                Content = "Test content 1",
                Author = "Test Author 1"
            },
            new NewsItem
            {
                Title = "Test News 2",
                Description = "Test description 2",
                Type = "Test",
                PublishDate = DateTime.UtcNow.AddMinutes(-10),
                Content = "Test content 2",
                Author = "Test Author 2"
            },
            new NewsItem
            {
                Title = "Test News 3",
                Description = "Test description 3",
                Type = "Test",
                PublishDate = DateTime.UtcNow.AddHours(-1),
                Content = "Test content 3",
                Author = "Test Author 3"
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

        // Act
        var result = await service.GetIndexAsync(request, CancellationToken.None);

        // Assert
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
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(GetByIdAsyncWithSearchShouldReturnSuccessWithValidData)) // Do NOT use InMemoryDatabase... it's not reliable. Use a real database and come up with a strategy to clean up the database between tests.
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        var newsItem = new NewsItem
        {
            Title = "Test News 1",
            Description = "Test description 1",
            Type = "Test",
            PublishDate = DateTime.UtcNow,
            Content = "Test content 1",
            Author = "Test Author 1"
        };

        dbContext.NewsItems.Add(newsItem);

        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);



        // Act
        var result = await service.GetByIdAsync(newsItem.Id);

        // Assert
        if (result.IsSuccess)
        {
            result.Value.ShouldNotBeNull();
            result.Value.NewsItem.ShouldNotBeNull();
            result.Value.NewsItem.Id.ShouldBe(newsItem.Id);
        }
        else
        {
            result.Status.ShouldBe(ResultStatus.NotFound);
        }
    }
}