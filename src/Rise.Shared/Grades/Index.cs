namespace Rise.Shared.Grades;

public static partial class GradesResponse
{
    public class CourseList
    {
        public IEnumerable<GradesDto.Course> Courses { get; set; } = [];
    }
    public class CourseById
    {
        public GradesDto.Course Course { get; set; } = default!;
    }

}

public static partial class GradesRequest
{
    public class CourseById
    {
        public required string CourseId { get; set; }
    }
}