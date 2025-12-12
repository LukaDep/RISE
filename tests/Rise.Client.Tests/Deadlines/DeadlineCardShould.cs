using Microsoft.Extensions.Localization;
using Rise.Client.Deadlines.Components;
using Rise.Shared.Deadlines;

namespace Rise.Client.Deadlines;

public class DeadlineCardShould : TestContext
{
    public DeadlineCardShould()
    {
        Services.AddLocalization();
    }

    private static DeadlineDto.Index CreateDeadline(
        string title = "Test Assignment",
        string lector = "Prof. Test",
        DateTime? endDate = null,
        string? description = null,
        string? course = null)
    {
        return new DeadlineDto.Index
        {
            Id = Guid.NewGuid(),
            Title = title,
            Lector = lector,
            EndDate = endDate ?? DateTime.Now.AddDays(5),
            Description = description,
            Course = course,
            UserId = "test-user-id"
        };
    }

    [Fact]
    public void DisplaysDeadlineTitle()
    {
        // Arrange
        var deadline = CreateDeadline(title: "Important Assignment");

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("Important Assignment", cut.Markup);
    }

    [Fact]
    public void DisplaysLectorName()
    {
        // Arrange
        var deadline = CreateDeadline(lector: "Prof. Johnson");

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("Prof. Johnson", cut.Markup);
        Assert.Contains("fa-user", cut.Markup);
    }

    [Fact]
    public void DisplaysCourse_WhenProvided()
    {
        // Arrange
        var deadline = CreateDeadline(course: "Mathematics 101");

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("Mathematics 101", cut.Markup);
        Assert.Contains("fa-book", cut.Markup);
    }

    [Fact]
    public void DoesNotDisplayCourse_WhenNotProvided()
    {
        // Arrange
        var deadline = CreateDeadline(course: null);

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert - should not contain course book icon in course context
        // The component only shows course section when course is provided
        var markup = cut.Markup;
        Assert.DoesNotContain("bg-hogent-education/10", markup); // Course badge styling
    }

    [Fact]
    public void DisplaysDescription_WhenProvided()
    {
        // Arrange
        var deadline = CreateDeadline(description: "Complete all exercises from chapter 5");

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("Complete all exercises from chapter 5", cut.Markup);
    }

    [Fact]
    public void DisplaysEndDate()
    {
        // Arrange
        var endDate = new DateTime(2025, 12, 25, 14, 0, 0);
        var deadline = CreateDeadline(endDate: endDate);

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("25", cut.Markup); // Day
        Assert.Contains("2025", cut.Markup); // Year
    }

    [Fact]
    public void ShowsExpiredState_WhenDeadlineIsInPast()
    {
        // Arrange
        var deadline = CreateDeadline(endDate: DateTime.Now.AddDays(-2));

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Deadlines.Expired"], cut.Markup);
        Assert.Contains("fa-clock-rotate-left", cut.Markup);
        Assert.Contains("border-dashed", cut.Markup); // Expired cards have dashed border
        Assert.Contains("line-through", cut.Markup); // Title is struck through
    }

    [Fact]
    public void ShowsRedIndicator_WhenDeadlineIsWithin1Day()
    {
        // Arrange
        var deadline = CreateDeadline(endDate: DateTime.Now.AddHours(12));

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("bg-red-500", cut.Markup); // Red left border indicator
    }

    [Fact]
    public void ShowsOrangeIndicator_WhenDeadlineIsWithin3Days()
    {
        // Arrange
        var deadline = CreateDeadline(endDate: DateTime.Now.AddDays(2));

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("bg-orange-400", cut.Markup); // Orange left border indicator
    }

    [Fact]
    public void ShowsYellowIndicator_WhenDeadlineIsWithin7Days()
    {
        // Arrange
        var deadline = CreateDeadline(endDate: DateTime.Now.AddDays(5));

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("bg-yellow-400", cut.Markup); // Yellow left border indicator
    }

    [Fact]
    public void ShowsDefaultIndicator_WhenDeadlineIsMoreThan7DaysAway()
    {
        // Arrange
        var deadline = CreateDeadline(endDate: DateTime.Now.AddDays(10));

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("bg-hogent-education", cut.Markup); // Default education color
    }

    [Fact]
    public void ShowsUrgentBadge_WhenDeadlineIsToday()
    {
        // Arrange - deadline later today (DaysRemaining == 0)
        var deadline = CreateDeadline(endDate: DateTime.Now.AddMinutes(30));

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert - when DaysRemaining is 0 or 1, show urgent indicator
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        // Should have either fire (today) or exclamation (1 day) icon
        var hasUrgencyIndicator = cut.Markup.Contains("fa-fire") || cut.Markup.Contains("fa-exclamation");
        Assert.True(hasUrgencyIndicator, "Expected urgent badge with fire or exclamation icon");
    }

    [Fact]
    public void ShowsDaysLeftBadge_WhenDeadlineIsWithinWeek()
    {
        // Arrange
        var deadline = CreateDeadline(endDate: DateTime.Now.AddDays(4));

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("fa-clock", cut.Markup); // Clock icon for days left
    }

    [Fact]
    public void DoesNotShowUrgencyBadge_WhenDeadlineIsMoreThan7DaysAway()
    {
        // Arrange
        var deadline = CreateDeadline(endDate: DateTime.Now.AddDays(14));

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert - should not have urgency indicators
        Assert.DoesNotContain("fa-fire", cut.Markup);
        Assert.DoesNotContain("bg-red-100", cut.Markup);
        Assert.DoesNotContain("bg-orange-100", cut.Markup);
        Assert.DoesNotContain("bg-yellow-100", cut.Markup);
    }

    [Fact]
    public void DisplaysDeadlineLabel()
    {
        // Arrange
        var deadline = CreateDeadline();

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Deadlines.Deadline"], cut.Markup);
    }

    [Fact]
    public void ShowsExpiredDaysAgo_WhenDeadlineExpiredMultipleDays()
    {
        // Arrange
        var deadline = CreateDeadline(endDate: DateTime.Now.AddDays(-5));

        // Act
        var cut = RenderComponent<DeadlineCard>(parameters => parameters
            .Add(p => p.Deadline, deadline));

        // Assert
        Assert.Contains("bg-red-50", cut.Markup); // Expired badge container
        Assert.Contains("text-red-600", cut.Markup); // Expired text styling
    }
}
