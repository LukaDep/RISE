namespace Rise.Shared.News;



public static partial class NewsResponse
{
    public class Index
    {
        public IEnumerable<NewsDto.Index> News { get; set; } = [];
    }
}


