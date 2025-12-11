namespace Rise.Client.Grades;

using Rise.Shared.Grades;
using Rise.Client.Grades.Components;

public class GradeItemExtraShould : TestContext
{
    public GradeItemExtraShould()
    {
        Services.AddLocalization();
    }

    [Fact]
    public void ShowsFeedbackAfterClick_WhenFeedbackPresent()
    {
        var grade = new GradesDto.Grade
        {
            Id = Guid.NewGuid(),
            Name = "Quiz",
            ActivityType = "Test",
            CourseName = "CS",
            Year = "2025",
            Semester = 1,
            MaxPoints = 10,
            Score = 8,
            Date = DateTime.Now.AddDays(-1),
            Feedback = "Well done"
        };

        var cut = RenderComponent<GradeItem>(parameters => parameters.Add(p => p.Grade, grade));

        Assert.DoesNotContain("Well done", cut.Markup);

        // click to toggle details
        cut.Find("div").Click();

        Assert.Contains("Well done", cut.Markup);
    }

    [Fact]
    public void UsesLowBackgroundWhenScoreLessThanHalf()
    {
        var grade = new GradesDto.Grade
        {
            Id = Guid.NewGuid(),
            Name = "Poor",
            ActivityType = "Exam",
            CourseName = "CS",
            Year = "2025",
            Semester = 1,
            MaxPoints = 100,
            Score = 40,
            Date = DateTime.Now
        };

        var cut = RenderComponent<GradeItem>(parameters => parameters.Add(p => p.Grade, grade));

        // low score should render a failing badge (red)
        Assert.Contains("bg-red-500", cut.Markup);
        Assert.Contains("text-white", cut.Markup);
    }

    [Fact]
    public void UsesDefaultBackgroundWhenScoreMissingOrMaxZero()
    {
        var grade = new GradesDto.Grade
        {
            Id = Guid.NewGuid(),
            Name = "NoScore",
            ActivityType = "Exam",
            CourseName = "CS",
            Year = "2025",
            Semester = 1,
            MaxPoints = null,
            Score = null,
            Date = DateTime.Now
        };

        var cut = RenderComponent<GradeItem>(parameters => parameters.Add(p => p.Grade, grade));

        // when score/max missing, badge uses neutral styling
        Assert.Contains("bg-hogent-black-10", cut.Markup);
        Assert.Contains("text-hogent-black", cut.Markup);
    }
}
