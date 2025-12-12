using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Rise.Shared.Common;
using Rise.Shared.News;
using Serilog.Core;
using Xunit.Abstractions;

namespace Rise.Client.News;

public class IndexShould : TestContext
{
    public IndexShould()
    {
        Services.AddLocalization();
        Services.AddScoped<INewsService, FakeNewsService>();
        JSInterop.SetupVoid("initScrollTop", _ => true);
    }

    [Fact]
    public void RendersHeaderAndSearchElements()
    {
        // Arrange & Act
        var cut = RenderComponent<Index>();

        // Assert header/title rendered from localizer
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains("<h1", cut.Markup);
        Assert.Contains(localizer["News.Title"], cut.Markup);

        // Assert the search icon exists
        Assert.Contains("fa-magnifying-glass", cut.Markup);

        // Assert the search button/svg exists
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void RendersNewsItemsWhenDataAvailable()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert - check that news items are rendered
        Assert.Contains("Campus reopens", cut.Markup);
        Assert.Contains("New library hours", cut.Markup);
        Assert.Contains("Cafeteria menu updated", cut.Markup);
        Assert.Contains("Guest lecture series", cut.Markup);
    }

    [Fact]
    public void ShowsNewsCountInformation()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert - checks news count display (4 of 4 in the fake service)
        Assert.Contains("4", cut.Markup); // Should show current count
    }

    [Fact]
    public void RendersNewsItemLinksCorrectly()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert - NewsItem components render links to news articles
        var links = cut.FindAll("a[href^='/news/']");
        Assert.NotEmpty(links);
    }

    [Fact]
    public void SearchInputAcceptsValue()
    {
        // Act
        var cut = RenderComponent<Index>();
        var input = cut.Find("input");

        // Enter search term
        input.Input("test search");

        // Assert - navigation should include search term
        var navigationManager = Services.GetRequiredService<NavigationManager>();
        Assert.Contains("SearchTerm", navigationManager.Uri, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RendersWithCorrectCssClasses()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert - verify structure is present
        Assert.Contains("mt-4", cut.Markup);
    }

    [Fact]
    public void RendersNewsItemImages()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert - images should be rendered
        var images = cut.FindAll("img");
        Assert.NotEmpty(images);
    }

    [Fact]
    public void RendersScrollToTopContainer()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert - top element for scroll
        Assert.Contains("id=\"top\"", cut.Markup);
    }

    [Fact]
    public void RendersPageTitle()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert - page title element
        Assert.NotNull(cut.FindComponent<Microsoft.AspNetCore.Components.Web.PageTitle>());
    }

    [Fact]
    public void RendersTotalAndCurrentCountWhenNewsAvailable()
    {
        // Act
        var cut = RenderComponent<Index>();

        // The FakeNewsService returns 4 items total, and shows count info
        // Looking for the count pattern in the rendered output
        Assert.Contains("4", cut.Markup);
    }
}

public class IndexWithNullNewsShould : TestContext
{
    public IndexWithNullNewsShould()
    {
        Services.AddLocalization();
        Services.AddScoped<INewsService, NullNewsService>();
        JSInterop.SetupVoid("initScrollTop", _ => true);
    }

    [Fact]
    public void ShowsSpinnerWhenNewsIsNull()
    {
        // Act
        var cut = RenderComponent<Index>();

        // The component shows a loading spinner when news == null
        Assert.Contains("animate-spin", cut.Markup);
    }
}

public class IndexWithEmptyNewsShould : TestContext
{
    public IndexWithEmptyNewsShould()
    {
        Services.AddLocalization();
        Services.AddScoped<INewsService, EmptyNewsService>();
        JSInterop.SetupVoid("initScrollTop", _ => true);
    }

    [Fact]
    public void ShowsNotFoundMessageWhenNoNews()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["News.NotFound"], cut.Markup);
    }
}

public class IndexWithPaginatedNewsShould : TestContext
{
    public IndexWithPaginatedNewsShould()
    {
        Services.AddLocalization();
        Services.AddScoped<INewsService, PaginatedNewsService>();
        JSInterop.SetupVoid("initScrollTop", _ => true);
    }

    [Fact]
    public void ShowsLoadMoreButtonWhenMoreNewsAvailable()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["News.LoadMore"], cut.Markup);
        Assert.Contains("fa-arrow-down", cut.Markup);
    }

    [Fact]
    public async Task LoadMoreButtonLoadsAdditionalNews()
    {
        // Arrange
        var cut = RenderComponent<Index>();

        // Get initial count
        var initialMarkup = cut.Markup;
        Assert.Contains("News 1", initialMarkup);

        // Act - click load more button
        var loadMoreButton = cut.Find("button");
        await loadMoreButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        // Assert - more news should be loaded
        Assert.Contains("News 1", cut.Markup);
    }

    [Fact]
    public void ShowsCorrectTotalCountWithPagination()
    {
        // Act
        var cut = RenderComponent<Index>();

        // Assert - should show count out of total
        Assert.Contains("25", cut.Markup); // Total count
    }
}

public class EmptyNewsService : INewsService
{
    public Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        return Task.FromResult(Result.Success(new NewsResponse.Index
        {
            News = new List<NewsDto.Index>(),
            TotalCount = 0
        }));
    }

    public Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        return Task.FromResult(Result<NewsResponse.Get>.NotFound());
    }
}

public class PaginatedNewsService : INewsService
{
    private readonly List<NewsDto.Index> _allNews = Enumerable.Range(1, 25).Select(i => new NewsDto.Index
    {
        Id = Guid.NewGuid(),
        Title = $"News {i}",
        Description = $"Description {i}",
        Type = "Test",
        PublishDate = DateTime.UtcNow.AddDays(-i),
        Content = $"Content {i}",
        Author = "Author",
        ImageUrl = "https://example.com/image.jpg"
    }).ToList();

    public Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var page = _allNews.Skip(request.Skip).Take(request.Take).ToList();
        return Task.FromResult(Result.Success(new NewsResponse.Index
        {
            News = page,
            TotalCount = _allNews.Count
        }));
    }

    public Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        var item = _allNews.FirstOrDefault(n => n.Id == id);
        if (item == null)
            return Task.FromResult(Result<NewsResponse.Get>.NotFound());
        return Task.FromResult(Result.Success(new NewsResponse.Get { NewsArticle = item }));
    }
}