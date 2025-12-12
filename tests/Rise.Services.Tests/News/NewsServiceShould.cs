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
                ImageUrl = "https://example.com/image2.jpg"
            },
            new NewsArticle
            {
                Title = "Test News 3",
                Description = "Test description 3",
                Type = "Test",
                PublishDate = DateTime.Now.AddHours(-1),
                Content = "Test content 3",
                Author = "Test Author 3",
                ImageUrl = "https://example.com/image3.jpg"
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
                ImageUrl = "https://example.com/image2.jpg"
            },
            new NewsArticle
            {
                Title = "Test News 3",
                Description = "Test description 3",
                Type = "Test",
                PublishDate = DateTime.Now.AddHours(-1),
                Content = "Test content 3",
                Author = "Test Author 3",
                ImageUrl = "https://example.com/image3.jpg"
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

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        var service = new NewsService(dbContext);

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetIndexAsync_WithSearchByAuthor_ShouldFilterResults()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        dbContext.NewsArticles.AddRange(
            new NewsArticle
            {
                Title = "News A",
                Description = "Description A",
                Type = "Test",
                PublishDate = DateTime.Now,
                Content = "Content A",
                Author = "John Smith",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "News B",
                Description = "Description B",
                Type = "Test",
                PublishDate = DateTime.Now,
                Content = "Content B",
                Author = "Jane Doe",
                ImageUrl = "https://example.com/image2.jpg"
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act
        var request = new QueryRequest.DateRange { Skip = 0, Take = 10, SearchTerm = "John" };
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.News);
        Assert.Equal("News A", result.Value.News.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_WithDateRange_ShouldFilterByDates()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var today = DateTime.UtcNow.Date;
        dbContext.NewsArticles.AddRange(
            new NewsArticle
            {
                Title = "Today News",
                Description = "Today description",
                Type = "Test",
                PublishDate = today.AddHours(10),
                Content = "Today content",
                Author = "Author",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "Old News",
                Description = "Old description",
                Type = "Test",
                PublishDate = today.AddDays(-30),
                Content = "Old content",
                Author = "Author",
                ImageUrl = "https://example.com/image2.jpg"
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act
        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10,
            StartDate = today,
            EndDate = today
        };
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.News);
        Assert.Equal("Today News", result.Value.News.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_WithStartDateOnly_ShouldFilterFromStartDate()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var today = DateTime.UtcNow.Date;
        dbContext.NewsArticles.AddRange(
            new NewsArticle
            {
                Title = "Recent News",
                Description = "Recent description",
                Type = "Test",
                PublishDate = today.AddDays(-5),
                Content = "Recent content",
                Author = "Author",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "Old News",
                Description = "Old description",
                Type = "Test",
                PublishDate = today.AddDays(-30),
                Content = "Old content",
                Author = "Author",
                ImageUrl = "https://example.com/image2.jpg"
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act
        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10,
            StartDate = today.AddDays(-10)
        };
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.News);
        Assert.Equal("Recent News", result.Value.News.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_WithEndDateOnly_ShouldFilterUntilEndDate()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var today = DateTime.UtcNow.Date;
        dbContext.NewsArticles.AddRange(
            new NewsArticle
            {
                Title = "Recent News",
                Description = "Recent description",
                Type = "Test",
                PublishDate = today,
                Content = "Recent content",
                Author = "Author",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "Old News",
                Description = "Old description",
                Type = "Test",
                PublishDate = today.AddDays(-30),
                Content = "Old content",
                Author = "Author",
                ImageUrl = "https://example.com/image2.jpg"
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act
        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10,
            EndDate = today.AddDays(-20)
        };
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.News);
        Assert.Equal("Old News", result.Value.News.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_WithSwappedDateRange_ShouldAutoCorrectDates()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var today = DateTime.UtcNow.Date;
        dbContext.NewsArticles.Add(new NewsArticle
        {
            Title = "News in Range",
            Description = "Description",
            Type = "Test",
            PublishDate = today.AddDays(-5),
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image1.jpg"
        });
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act - swap start and end dates
        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10,
            StartDate = today, // End date
            EndDate = today.AddDays(-10) // Start date
        };
        var result = await service.GetIndexAsync(request);

        // Assert - should still find the news (dates are auto-swapped)
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.News);
    }

    [Fact]
    public async Task GetIndexAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        for (int i = 1; i <= 15; i++)
        {
            dbContext.NewsArticles.Add(new NewsArticle
            {
                Title = $"News {i}",
                Description = $"Description {i}",
                Type = "Test",
                PublishDate = DateTime.Now.AddMinutes(-i),
                Content = $"Content {i}",
                Author = "Author",
                ImageUrl = "https://example.com/image.jpg"
            });
        }
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act
        var request = new QueryRequest.DateRange { Skip = 5, Take = 5 };
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value.News.Count());
        Assert.Equal(15, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetIndexAsync_WithOrderBy_ShouldOrderResults()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        dbContext.NewsArticles.AddRange(
            new NewsArticle
            {
                Title = "Alpha News",
                Description = "Description A",
                Type = "Test",
                PublishDate = DateTime.Now.AddDays(-1),
                Content = "Content A",
                Author = "Author",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "Zeta News",
                Description = "Description Z",
                Type = "Test",
                PublishDate = DateTime.Now,
                Content = "Content Z",
                Author = "Author",
                ImageUrl = "https://example.com/image2.jpg"
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act
        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10,
            OrderBy = "Title",
            OrderDescending = false
        };
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Alpha News", result.Value.News.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_WithOrderByDescending_ShouldOrderResultsDescending()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        dbContext.NewsArticles.AddRange(
            new NewsArticle
            {
                Title = "Alpha News",
                Description = "Description A",
                Type = "Test",
                PublishDate = DateTime.Now.AddDays(-1),
                Content = "Content A",
                Author = "Author",
                ImageUrl = "https://example.com/image1.jpg"
            },
            new NewsArticle
            {
                Title = "Zeta News",
                Description = "Description Z",
                Type = "Test",
                PublishDate = DateTime.Now,
                Content = "Content Z",
                Author = "Author",
                ImageUrl = "https://example.com/image2.jpg"
            }
        );
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act
        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10,
            OrderBy = "Title",
            OrderDescending = true
        };
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Zeta News", result.Value.News.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_WithDefaultDateValues_ShouldIgnoreDateFilters()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        dbContext.NewsArticles.Add(new NewsArticle
        {
            Title = "Test News",
            Description = "Description",
            Type = "Test",
            PublishDate = DateTime.Now,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act - pass default DateTime values
        var request = new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 10,
            StartDate = default(DateTime),
            EndDate = default(DateTime)
        };
        var result = await service.GetIndexAsync(request);

        // Assert - should return news (default dates are treated as not provided)
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.News);
    }

    [Fact]
    public async Task GetIndexAsync_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();
        var service = new NewsService(dbContext);

        // Act
        var request = new QueryRequest.DateRange { Skip = 0, Take = 10 };
        var result = await service.GetIndexAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.News);
        Assert.Equal(0, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAllProperties()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var expectedNews = new NewsArticle
        {
            Title = "Full News Article",
            Description = "Full Description",
            Type = "Announcement",
            PublishDate = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc),
            Content = "Full Content",
            Author = "Full Author",
            ImageUrl = "https://example.com/full-image.jpg"
        };
        dbContext.NewsArticles.Add(expectedNews);
        await dbContext.SaveChangesAsync();

        var service = new NewsService(dbContext);

        // Act
        var result = await service.GetByIdAsync(expectedNews.Id);

        // Assert
        Assert.True(result.IsSuccess);
        var article = result.Value.NewsArticle;
        Assert.Equal(expectedNews.Id, article.Id);
        Assert.Equal("Full News Article", article.Title);
        Assert.Equal("Full Description", article.Description);
        Assert.Equal("Announcement", article.Type);
        Assert.Equal("Full Content", article.Content);
        Assert.Equal("Full Author", article.Author);
        Assert.Equal("https://example.com/full-image.jpg", article.ImageUrl);
    }
}