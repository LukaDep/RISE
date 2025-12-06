namespace Rise.Domain.News;

public class NewsArticle : Entity
{
    private string _title = string.Empty;

    public required string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }
    private string? _description = string.Empty;
    public string? Description
    {
        get => _description;
        set => _description = value != null ? Guard.Against.NullOrWhiteSpace(value) : null;
    }
    private string _type = string.Empty;
    public required string Type
    {
        get => _type;
        set => _type = Guard.Against.NullOrWhiteSpace(value);
    }
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
    private string _content = string.Empty;
    public required string Content
    {
        get => _content;
        set => _content = Guard.Against.NullOrWhiteSpace(value);
    }
    private string _author = string.Empty;
    public required string Author
    {
        get => _author;
        set => _author = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _imageUrl = string.Empty;

    public required string ImageUrl
    {
        get => _imageUrl;
        set => _imageUrl = Guard.Against.NullOrWhiteSpace(value);
    }
}