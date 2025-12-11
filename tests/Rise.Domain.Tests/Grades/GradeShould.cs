using Rise.Domain.Grades;

namespace Rise.Domain.Tests.Grades;

/// Example Domain Tests using xUnit and Shouldly.
/// See <seealso href="https://xunit.net"/> and <seealso href="https://docs.shouldly.org"/>.
/// </summary>
/// <seealso href="https://xunit.net/?tabs=cs"/>
public class GradeShould
{
    private readonly DateTime _date = DateTime.Now.ToUniversalTime();

    [Fact]
    public void BeCreated()
    {
        var p = new Grade { CourseName = "Mathematics", CourseId = "Course1", Year = "2025", Semester = 1, Name = "Midterm Exam", ActivityType = "Exam", MaxPoints = 100, Score = 85, Date = _date.AddDays(-30) };


        p.Name.ShouldBe("Midterm Exam");
        p.CourseName.ShouldBe("Mathematics");
        p.CourseId.ShouldBe("Course1");
        p.Year.ShouldBe("2025");
        p.Semester.ShouldBe(1);
        p.ActivityType.ShouldBe("Exam");
        p.MaxPoints.ShouldBe(100);
        p.Score.ShouldBe(85);
        p.Date.ToUniversalTime().ShouldBe(_date.AddDays(-30));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidName(string? name)
    {
        Action act = () =>
        {
            new Grade()
            {
                Name = name!,
                ActivityType = "Exam",
                CourseName = "Mathematics",
                CourseId = "Course1",
                Year = "2025",
                Semester = 1,
                MaxPoints = 100,
                Score = 85,
                Date = _date.AddDays(-30)
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeChangedToHaveAnInvalidName(string? name)
    {
        Action act = () =>
        {
            // Create a valid grade first, then attempt to change its Name to an invalid value
            Grade grade = new()
            {
                Name = "Initial Valid Name",
                ActivityType = "Exam",
                CourseName = "Mathematics",
                CourseId = "Course1",
                Year = "2025",
                Semester = 1,
                MaxPoints = 100,
                Score = 85,
                Date = _date.AddDays(-30)
            };

            // Now attempt to set an invalid name
            grade.Name = name!;
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidActivityType(string? activityType)
    {
        Action act = () =>
        {
            new Grade()
            {
                Name = "Midterm Exam",
                ActivityType = activityType!,
                CourseName = "Mathematics",
                CourseId = "Course1",
                Year = "2025",
                Semester = 1,
                MaxPoints = 100,
                Score = 85,
                Date = _date.AddDays(-30)
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeChangedToHaveAnInvalidActivityType(string? activityType)
    {
        Action act = () =>
        {
            // Create a valid grade first, then attempt to change its ActivityType to an invalid value
            Grade grade = new()
            {
                Name = "Midterm Exam",
                ActivityType = "Exam",
                CourseName = "Mathematics",
                CourseId = "Course1",
                Year = "2025",
                Semester = 1,
                MaxPoints = 100,
                Score = 85,
                Date = _date.AddDays(-30)
            };

            // Now attempt to set an invalid activity type
            grade.ActivityType = activityType!;
        };

        act.ShouldThrow<ArgumentException>();
    }

}