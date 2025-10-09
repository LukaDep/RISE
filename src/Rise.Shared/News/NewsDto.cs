namespace Rise.Shared.News;

public static class NewsDto
{
    public class Index
    {
        public required int Id { get; set; }
        public required String Title { get; set; }
        public required DateTime PublishDate { get; set; }
        public required String Content { get; set; }
        public required String Author { get; set; }
    }
}