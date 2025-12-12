namespace Rise.Shared.Grades;

/// <summary>
/// Data transfer objects for student grades and results.
/// </summary>
public static class GradesDto
{
    /// <summary>
    /// Represents a grade record for display and retrieval.
    /// Contains grade details (name, activity type, score, feedback) and associated course information.
    /// </summary>
    public class Grade
    {
        // Grade details
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ActivityType { get; set; }
        public double? MaxPoints { get; set; }
        public double? Score { get; set; }
        public string? Feedback { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime Date { get; set; }

        // Course details
        public string? CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? Year { get; set; }
        public int? Semester { get; set; }
    }
}