using Rise.Client.News.Components;
using Rise.Shared.News;

namespace Rise.Client.News;

public class NewsItemShould : TestContext
{
    public NewsItemShould()
    {
        Services.AddLocalization();
    }

    [Fact]
    public void RenderNewsItemWithAllProperties()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "Test News Title",
            Description = "This is a test description for the news item.",
            Type = "Announcement",
            PublishDate = new DateTime(2024, 6, 15),
            Content = "Full content here",
            Author = "Test Author",
            ImageUrl = "https://example.com/test-image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert
        Assert.Contains("Test News Title", cut.Markup);
        Assert.Contains("This is a test description for the news item.", cut.Markup);
        Assert.Contains("June 15, 2024", cut.Markup);
    }

    [Fact]
    public void RenderLinkToNewsArticle()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var newsDto = new NewsDto.Index
        {
            Id = newsId,
            Title = "Test News",
            Description = "Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert
        var link = cut.Find("a");
        Assert.Contains($"/news/{newsId}", link.GetAttribute("href"));
    }

    [Fact]
    public void RenderImageWithCorrectSrc()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "Test News",
            Description = "Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/specific-image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert
        var img = cut.Find("img");
        Assert.Equal("https://example.com/specific-image.jpg", img.GetAttribute("src"));
    }

    [Fact]
    public void RenderImageWithAltText()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "Image Alt Text Test",
            Description = "Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert
        var img = cut.Find("img");
        Assert.Equal("Image Alt Text Test", img.GetAttribute("alt"));
    }

    [Fact]
    public void RenderPlaceholderWhenNoImage()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "No Image News",
            Description = "Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "" // Empty image URL
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert - should render placeholder div instead of image
        Assert.Contains("bg-hogent-black-50", cut.Markup);
    }

    [Fact]
    public void RenderDescriptionWithTruncation()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "Test News",
            Description = "This is a very long description that should be truncated by the line-clamp CSS class applied to the paragraph element.",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert - check that line-clamp is applied
        Assert.Contains("line-clamp-3", cut.Markup);
    }

    [Fact]
    public void RenderPublishDateFormatted()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "Test News",
            Description = "Description",
            Type = "News",
            PublishDate = new DateTime(2024, 12, 25),
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert - date should be formatted as "MMMM dd, yyyy"
        Assert.Contains("December 25, 2024", cut.Markup);
    }

    [Fact]
    public void RenderWithHoverStyles()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "Hover Test",
            Description = "Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert - check for hover styles
        Assert.Contains("hover:border-hogent-education", cut.Markup);
        Assert.Contains("hover:shadow-lg", cut.Markup);
    }

    [Fact]
    public void RenderWithCorrectStyling()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "Styling Test",
            Description = "Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert - verify key styling classes
        Assert.Contains("rounded-xl", cut.Markup);
        Assert.Contains("shadow", cut.Markup);
        Assert.Contains("cursor-pointer", cut.Markup);
    }

    [Fact]
    public void RenderTitleWithBoldStyling()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "Bold Title Test",
            Description = "Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert
        var h3 = cut.Find("h3");
        Assert.Contains("font-bold", h3.ClassName);
        Assert.Contains("Bold Title Test", h3.TextContent);
    }

    [Fact]
    public void RenderDataNewsIdAttribute()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var newsDto = new NewsDto.Index
        {
            Id = newsId,
            Title = "Data Attribute Test",
            Description = "Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert
        var link = cut.Find("a");
        Assert.Equal(newsId.ToString(), link.GetAttribute("data-news-id"));
    }

    [Fact]
    public void RenderLoadingSpinnerWhileImageLoads()
    {
        // Arrange
        var newsDto = new NewsDto.Index
        {
            Id = Guid.NewGuid(),
            Title = "Loading Test",
            Description = "Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var cut = RenderComponent<NewsItem>(p => p.Add(x => x.newsItem, newsDto));

        // Assert - spinner should be present initially (image not yet loaded)
        Assert.Contains("animate-spin", cut.Markup);
    }
}
