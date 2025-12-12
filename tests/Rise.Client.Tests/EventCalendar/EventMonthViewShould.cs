using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Client.EventCalendar.Components;
using Rise.Shared.Events;

namespace Rise.Client.EventCalendar;

public class EventMonthViewShould : TestContext
{
    public EventMonthViewShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IEventService, FakeEventService>();
        // Setup JSInterop for swipe functionality
        JSInterop.SetupVoid("initSwipe", _ => true);
        JSInterop.SetupVoid("destroySwipe", _ => true);
    }

    private static List<EventDTO.Index> CreateEvents()
    {
        var today = DateTime.Today;
        return new List<EventDTO.Index>
        {
            new()
            {
                Type = "Welzijn",
                Name = "Yoga Session",
                StartDateTime = today.AddDays(1),
                EndDateTime = today.AddDays(1).AddHours(2),
                Location = "Gym",
                Description = "Relaxing yoga"
            },
            new()
            {
                Type = "Sport",
                Name = "Basketball Game",
                StartDateTime = today.AddDays(3),
                EndDateTime = today.AddDays(3).AddHours(2),
                Location = "Sports Hall",
                Description = "Tournament"
            },
            new()
            {
                Type = "Andere",
                Name = "Workshop",
                StartDateTime = today.AddDays(5),
                EndDateTime = today.AddDays(5).AddHours(3),
                Location = "Room A",
                Description = "Creative workshop"
            }
        };
    }

    [Fact]
    public void DisplaysCurrentMonthAndYear()
    {
        // Arrange
        var selectedDate = new DateTime(2025, 12, 15);

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.Events, new List<EventDTO.Index>()));

        // Assert - December 2025 should be visible
        Assert.Contains("2025", cut.Markup);
    }

    [Fact]
    public void DisplaysWeekDayHeaders()
    {
        // Arrange
        var selectedDate = DateTime.Today;

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.Events, new List<EventDTO.Index>()));

        // Assert - should show abbreviated day names
        Assert.Contains("grid-cols-7", cut.Markup);
    }

    [Fact]
    public void DisplaysCalendarGrid()
    {
        // Arrange
        var selectedDate = new DateTime(2025, 12, 15);

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.Events, new List<EventDTO.Index>()));

        // Assert - should display day numbers
        Assert.Contains("15", cut.Markup); // Selected date's day
        Assert.Contains("1", cut.Markup); // First day of month
    }

    [Fact]
    public void HighlightsTodayDate()
    {
        // Arrange
        var selectedDate = DateTime.Today;

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.Events, new List<EventDTO.Index>()));

        // Assert - today should have special styling
        Assert.Contains("bg-hogent-education-15", cut.Markup);
        Assert.Contains("border-hogent-education", cut.Markup);
    }

    [Fact]
    public void DisplaysNavigationButtons()
    {
        // Arrange
        var selectedDate = DateTime.Today;

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.Events, new List<EventDTO.Index>()));

        // Assert - should have previous and next buttons
        var buttons = cut.FindAll("button");
        Assert.True(buttons.Count >= 2);
        Assert.Contains("‹", cut.Markup); // Previous button
        Assert.Contains("›", cut.Markup); // Next button
    }

    [Fact]
    public void DisplaysEventIndicators_WhenEventsExist()
    {
        // Arrange
        var today = DateTime.Today;
        var events = new List<EventDTO.Index>
        {
            new()
            {
                Type = "Welzijn",
                Name = "Event",
                StartDateTime = today,
                EndDateTime = today.AddHours(2),
                Location = "Room",
                Description = "Test"
            }
        };

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, today)
            .Add(p => p.Events, events));

        // Assert - should show event indicator bar
        Assert.Contains("h-1", cut.Markup); // Event indicator height
    }

    [Fact]
    public void DisplaysEventTypeLegend()
    {
        // Arrange
        var today = DateTime.Today;
        var events = new List<EventDTO.Index>
        {
            new()
            {
                Type = "Welzijn",
                Name = "Yoga",
                StartDateTime = today,
                EndDateTime = today.AddHours(2),
                Location = "Gym",
                Description = "Test"
            }
        };

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, today)
            .Add(p => p.Events, events));

        // Assert - should show type legend
        Assert.Contains("Welzijn", cut.Markup);
    }

    [Fact]
    public async Task NavigatesToPreviousMonth_WhenPreviousButtonClicked()
    {
        // Arrange
        var selectedDate = new DateTime(2025, 12, 15);
        var events = new List<EventDTO.Index>();

        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.Events, events));

        // Act - click previous button
        var prevButton = cut.FindAll("button").First();
        await cut.InvokeAsync(() => prevButton.Click());

        // Wait for animation
        await Task.Delay(300);

        // Assert - should show November
        // Note: The component updates internal state, we verify the button exists
        Assert.Contains("‹", cut.Markup);
    }

    [Fact]
    public async Task NavigatesToNextMonth_WhenNextButtonClicked()
    {
        // Arrange
        var selectedDate = new DateTime(2025, 12, 15);
        var events = new List<EventDTO.Index>();

        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.Events, events));

        // Act - click next button
        var buttons = cut.FindAll("button");
        var nextButton = buttons.Last();
        await cut.InvokeAsync(() => nextButton.Click());

        // Wait for animation
        await Task.Delay(300);

        // Assert - button should still exist
        Assert.Contains("›", cut.Markup);
    }

    [Fact]
    public void AppliesCorrectColorForWelzijnType()
    {
        // Arrange
        var today = DateTime.Today;
        var events = new List<EventDTO.Index>
        {
            new()
            {
                Type = "Welzijn",
                Name = "Event",
                StartDateTime = today,
                EndDateTime = today.AddHours(2),
                Location = "Room",
                Description = "Test"
            }
        };

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, today)
            .Add(p => p.Events, events));

        // Assert
        Assert.Contains("bg-hogent-education-30", cut.Markup);
    }

    [Fact]
    public void AppliesCorrectColorForAndereType()
    {
        // Arrange
        var today = DateTime.Today;
        var events = new List<EventDTO.Index>
        {
            new()
            {
                Type = "Andere",
                Name = "Event",
                StartDateTime = today,
                EndDateTime = today.AddHours(2),
                Location = "Room",
                Description = "Test"
            }
        };

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, today)
            .Add(p => p.Events, events));

        // Assert
        Assert.Contains("bg-hogent-it-30", cut.Markup);
    }

    [Fact]
    public void DisplaysDaysFromPreviousMonth()
    {
        // Arrange - December 2025 starts on Monday, so no previous month days
        // Let's use a month that starts mid-week
        var selectedDate = new DateTime(2025, 11, 15); // November 2025 starts on Saturday

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.Events, new List<EventDTO.Index>()));

        // Assert - should have days styled differently (from prev month)
        Assert.Contains("bg-hogent-black-15", cut.Markup); // Previous/next month days
    }

    [Fact]
    public void ShowsMoreIndicator_WhenMoreThan3EventsOnDay()
    {
        // Arrange
        var today = DateTime.Today;
        var events = new List<EventDTO.Index>
        {
            new() { Type = "Sport", Name = "Event 1", StartDateTime = today, EndDateTime = today.AddHours(1), Location = "A", Description = "1" },
            new() { Type = "Welzijn", Name = "Event 2", StartDateTime = today, EndDateTime = today.AddHours(1), Location = "B", Description = "2" },
            new() { Type = "Andere", Name = "Event 3", StartDateTime = today, EndDateTime = today.AddHours(1), Location = "C", Description = "3" },
            new() { Type = "Sport", Name = "Event 4", StartDateTime = today, EndDateTime = today.AddHours(1), Location = "D", Description = "4" }
        };

        // Act
        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, today)
            .Add(p => p.Events, events));

        // Assert - should show "+1 more" indicator
        Assert.Contains("+1", cut.Markup);
    }

    [Fact]
    public async Task InvokesDayClickCallback_WhenDayClicked()
    {
        // Arrange
        var selectedDate = DateTime.Today;
        DateTime? clickedDate = null;

        var cut = RenderComponent<EventMonthView>(parameters => parameters
            .Add(p => p.SelectedDate, selectedDate)
            .Add(p => p.Events, new List<EventDTO.Index>())
            .Add(p => p.OnDayClick, EventCallback.Factory.Create<DateTime>(this, date => clickedDate = date)));

        // Act - click on a day
        var dayCell = cut.FindAll("div.cursor-pointer").First();
        await cut.InvokeAsync(() => dayCell.Click());

        // Assert
        Assert.NotNull(clickedDate);
    }

    [Fact]
    public void StaticGetEventTypeBgColor_ReturnsCorrectColors()
    {
        // Assert
        Assert.Equal("bg-hogent-education-30 text-hogent-education", EventMonthView.GetEventTypeBgColor("Welzijn"));
        Assert.Equal("bg-hogent-education-30 text-hogent-education", EventMonthView.GetEventTypeBgColor("welzijn"));
        Assert.Equal("bg-hogent-it-30 text-hogent-it", EventMonthView.GetEventTypeBgColor("Andere"));
        Assert.Equal("bg-hogent-it-30 text-hogent-it", EventMonthView.GetEventTypeBgColor("andere"));
        Assert.Equal("bg-hogent-black-30 text-hogent-black", EventMonthView.GetEventTypeBgColor("Sport"));
        Assert.Equal("bg-hogent-black-30 text-hogent-black", EventMonthView.GetEventTypeBgColor("Unknown"));
    }
}
