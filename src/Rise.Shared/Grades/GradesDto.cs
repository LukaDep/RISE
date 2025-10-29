namespace Rise.Shared.Grades;

public static class GradesDto
{
    // public class Course
    // {
    //     public required string CourseId { get; set; }
    //     public required string CourseName { get; set; }
    //     public required int Year { get; set; }
    //     public required int Semester { get; set; }
    //     public double? FinalScore { get; set; }
    //     public required List<Grade> Grades { get; set; }
    // }
    public class Grade
    {
        // grade
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string ActivityType { get; set; }
        public double? MaxPoints { get; set; }
        public double? Score { get; set; }
        public string? Feedback { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime Date { get; set; }

        // course
        public string? CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? Year { get; set; }
        public int? Semester { get; set; }
    }
}