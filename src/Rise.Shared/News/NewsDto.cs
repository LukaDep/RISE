namespace Rise.Shared.News;

public static class NewsDto
{
    public class Index
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required DateTime PublishDate { get; set; }
        public required string Content { get; set; }
        public required string Author { get; set; }
    }
}