namespace Rise.Shared.News;

/// <summary>
/// Data transfer objects for news articles.
/// </summary>
public static class NewsDto
{
    /// <summary>
    /// Represents a news article for display and retrieval.
    /// Contains title, content, author, publication date, and associated media.
    /// </summary>
    public class Index
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string? Description { get; set; }
        public required string Type { get; set; }
        public required DateTime PublishDate { get; set; }
        public required string Content { get; set; }
        public required string Author { get; set; }
        public required string ImageUrl { get; set; }
    }
}