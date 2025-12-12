namespace Rise.Shared.News;

/// <summary>
/// Response wrappers for news-related operations.
/// </summary>
public static partial class NewsResponse
{
    /// <summary>
    /// Response containing a paginated list of news articles.
    /// Includes the total count for pagination purposes.
    /// </summary>
    public class Index
    {
        public IEnumerable<NewsDto.Index> News { get; set; } = [];
        public int TotalCount { get; set; }
    }
}


