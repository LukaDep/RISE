namespace Rise.Domain.News;

/// <summary>
/// Represents a news article.
/// Contains article details including title, content, author, publication date, and associated media.
/// </summary>
public class NewsArticle : Entity
{
    /// <summary>
    /// The title of the news article.
    /// </summary>
    private string _title = string.Empty;

    public required string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }
    /// <summary>
    /// A short description or summary of the article.
    /// </summary>
    private string? _description = string.Empty;
    public string? Description
    {
        get => _description;
        set => _description = value != null ? Guard.Against.NullOrWhiteSpace(value) : null;
    }
    /// <summary>
    /// The type or category of the news article.
    /// </summary>
    private string _type = string.Empty;
    public required string Type
    {
        get => _type;
        set => _type = Guard.Against.NullOrWhiteSpace(value);
    }
    /// <summary>
    /// The date when the article was published. Stored in UTC.
    /// </summary>
    private DateTime _publishDate;
    public required DateTime PublishDate
    {
        get => _publishDate;
        //weet niet zeker of dit goed is/werkt
        set
        {
            Guard.Against.Default(value, nameof(PublishDate));
            _publishDate = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }
    }
    /// <summary>
    /// The full content/body of the news article.
    /// </summary>
    private string _content = string.Empty;
    public required string Content
    {
        get => _content;
        set => _content = Guard.Against.NullOrWhiteSpace(value);
    }
    /// <summary>
    /// The author of the news article.
    /// </summary>
    private string _author = string.Empty;
    public required string Author
    {
        get => _author;
        set => _author = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// URL to the article's featured image.
    /// </summary>
    private string _imageUrl = string.Empty;

    public required string ImageUrl
    {
        get => _imageUrl;
        set => _imageUrl = Guard.Against.NullOrWhiteSpace(value);
    }
}