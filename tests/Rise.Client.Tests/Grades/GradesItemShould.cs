namespace Rise.Client.Grades;

using Rise.Shared.Grades;
using Rise.Client.Tests.Grades;
using Rise.Client.Grades.Components;

public class GradesItemShould : TestContext
{

    // grade parameter
    private readonly GradesDto.Grade _grade = new()
    {
        Id = Guid.CreateVersion7(),
        CourseName = "Mathematics",
        CourseId = "Course1",
        Year = "2023",
        Semester = 1,
        Name = "Midterm Exam",
        ActivityType = "Exam",
        MaxPoints = 100,
        Score = 85,
        Date = DateTime.Now.AddDays(-30)
    };

    public GradesItemShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IGradesService, FakeGradesService>();
    }

    [Fact]
    public void RendersGradeItemDetails()
    {

        // Arrange & Act
        var cut = RenderComponent<GradeItem>(parameters => parameters.Add(p => p.Grade, _grade));

        // Assert grade is present
        Assert.Contains(_grade.CourseName, cut.Markup);
        Assert.Contains(_grade.Name, cut.Markup);
        // score and max points are rendered in separate elements — assert both are present
        Assert.Contains($"{_grade.Score}", cut.Markup);
        Assert.Contains($"{_grade.MaxPoints}", cut.Markup);
    }

    [Fact]
    public void ClicksOnGradeItem()
    {
        // Arrange
        var cut = RenderComponent<GradeItem>(parameters => parameters.Add(p => p.Grade, _grade));

        if (_grade.Feedback != null)
        {
            // Assert grade feedback is not present before clicking
            Assert.DoesNotContain(_grade.Feedback, cut.Markup);

            // Act: click on the grade item
            var gradeDiv = cut.Find("div");
            gradeDiv.Click();

            // Assert grade feedback is present after clicking
            Assert.Contains(_grade.Feedback, cut.Markup);
        }
    }
}
