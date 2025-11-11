namespace Rise.Shared.Grades;

public static partial class GradesResponse
{
    public class Index
    {
        public IEnumerable<GradesDto.Grade> Grades { get; set; } = [];
    }
    public class Get
    {
        public GradesDto.Grade Grade { get; set; } = default!;
    }

}
