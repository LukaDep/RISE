namespace Rise.Shared.Deadlines;

public static partial class DeadlineResponse
{
    public class Index
    {
        public IEnumerable<DeadlineDto.Index> Deadlines { get; set; } = [];
    }
}