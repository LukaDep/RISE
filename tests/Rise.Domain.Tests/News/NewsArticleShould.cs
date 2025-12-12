using Rise.Domain.News;

namespace Rise.Domain.Tests.News;

public class NewsArticleShould
{
    #region Title Tests
    
    [Fact]
    public void CreateNewsArticle_WithValidTitle_ShouldSetTitle()
    {
        // Arrange & Act
        var article = new NewsArticle
        {
            Title = "Valid Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Assert
        Assert.Equal("Valid Title", article.Title);
    }

    [Fact]
    public void CreateNewsArticle_WithNullTitle_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = null!,
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    [Fact]
    public void CreateNewsArticle_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    [Fact]
    public void CreateNewsArticle_WithWhitespaceTitle_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "   ",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    #endregion

    #region Description Tests

    [Fact]
    public void CreateNewsArticle_WithValidDescription_ShouldSetDescription()
    {
        // Arrange & Act
        var article = new NewsArticle
        {
            Title = "Title",
            Description = "Valid Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Assert
        Assert.Equal("Valid Description", article.Description);
    }

    [Fact]
    public void CreateNewsArticle_WithNullDescription_ShouldAllowNull()
    {
        // Arrange & Act
        var article = new NewsArticle
        {
            Title = "Title",
            Description = null,
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Assert
        Assert.Null(article.Description);
    }

    [Fact]
    public void CreateNewsArticle_WithEmptyDescription_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Description = "",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    [Fact]
    public void CreateNewsArticle_WithWhitespaceDescription_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Description = "   ",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    #endregion

    #region Type Tests

    [Fact]
    public void CreateNewsArticle_WithValidType_ShouldSetType()
    {
        // Arrange & Act
        var article = new NewsArticle
        {
            Title = "Title",
            Type = "Announcement",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Assert
        Assert.Equal("Announcement", article.Type);
    }

    [Fact]
    public void CreateNewsArticle_WithNullType_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = null!,
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    [Fact]
    public void CreateNewsArticle_WithEmptyType_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    [Fact]
    public void CreateNewsArticle_WithWhitespaceType_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "   ",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    #endregion

    #region PublishDate Tests

    [Fact]
    public void CreateNewsArticle_WithValidPublishDate_ShouldSetPublishDate()
    {
        // Arrange
        var publishDate = new DateTime(2024, 6, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var article = new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = publishDate,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Assert
        Assert.Equal(publishDate, article.PublishDate);
    }

    [Fact]
    public void CreateNewsArticle_WithLocalPublishDate_ShouldConvertToUtc()
    {
        // Arrange
        var localDate = new DateTime(2024, 6, 15, 10, 30, 0, DateTimeKind.Local);

        // Act
        var article = new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = localDate,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Assert
        Assert.Equal(DateTimeKind.Utc, article.PublishDate.Kind);
    }

    [Fact]
    public void CreateNewsArticle_WithDefaultPublishDate_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = default,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    #endregion

    #region Content Tests

    [Fact]
    public void CreateNewsArticle_WithValidContent_ShouldSetContent()
    {
        // Arrange & Act
        var article = new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "This is the article content with markdown **support**.",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Assert
        Assert.Equal("This is the article content with markdown **support**.", article.Content);
    }

    [Fact]
    public void CreateNewsArticle_WithNullContent_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = null!,
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    [Fact]
    public void CreateNewsArticle_WithEmptyContent_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    [Fact]
    public void CreateNewsArticle_WithWhitespaceContent_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "   ",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    #endregion

    #region Author Tests

    [Fact]
    public void CreateNewsArticle_WithValidAuthor_ShouldSetAuthor()
    {
        // Arrange & Act
        var article = new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "John Doe",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Assert
        Assert.Equal("John Doe", article.Author);
    }

    [Fact]
    public void CreateNewsArticle_WithNullAuthor_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = null!,
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    [Fact]
    public void CreateNewsArticle_WithEmptyAuthor_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    [Fact]
    public void CreateNewsArticle_WithWhitespaceAuthor_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "   ",
            ImageUrl = "https://example.com/image.jpg"
        });
    }

    #endregion

    #region ImageUrl Tests

    [Fact]
    public void CreateNewsArticle_WithValidImageUrl_ShouldSetImageUrl()
    {
        // Arrange & Act
        var article = new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/news-image.jpg"
        };

        // Assert
        Assert.Equal("https://example.com/news-image.jpg", article.ImageUrl);
    }

    [Fact]
    public void CreateNewsArticle_WithNullImageUrl_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = null!
        });
    }

    [Fact]
    public void CreateNewsArticle_WithEmptyImageUrl_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = ""
        });
    }

    [Fact]
    public void CreateNewsArticle_WithWhitespaceImageUrl_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new NewsArticle
        {
            Title = "Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "   "
        });
    }

    #endregion

    #region Property Update Tests

    [Fact]
    public void UpdateTitle_WithValidValue_ShouldUpdateTitle()
    {
        // Arrange
        var article = new NewsArticle
        {
            Title = "Original Title",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        article.Title = "Updated Title";

        // Assert
        Assert.Equal("Updated Title", article.Title);
    }

    [Fact]
    public void UpdateDescription_WithValidValue_ShouldUpdateDescription()
    {
        // Arrange
        var article = new NewsArticle
        {
            Title = "Title",
            Description = "Original Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        article.Description = "Updated Description";

        // Assert
        Assert.Equal("Updated Description", article.Description);
    }

    [Fact]
    public void UpdateDescription_ToNull_ShouldAllowNull()
    {
        // Arrange
        var article = new NewsArticle
        {
            Title = "Title",
            Description = "Original Description",
            Type = "News",
            PublishDate = DateTime.UtcNow,
            Content = "Content",
            Author = "Author",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        article.Description = null;

        // Assert
        Assert.Null(article.Description);
    }

    #endregion
}
