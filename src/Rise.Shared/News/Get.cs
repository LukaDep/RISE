using System;

namespace Rise.Shared.News;

public static partial class NewsResponse
{
    /// <summary>
    /// Response containing a single news article.
    /// Used for detail views of individual news items.
    /// </summary>
    public class Get
    {
        public required NewsDto.Index NewsArticle { get; set; }
    }
}
