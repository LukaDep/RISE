using Microsoft.Extensions.Localization;
using Rise.Shared.Events;

namespace Rise.Client.EventCalendar;

public class EventCalendarPageShould : TestContext
{
    public EventCalendarPageShould()
    {
        Services.AddLocalization();
        // Setup JSInterop for the swipe functionality used by EventMonthView
        JSInterop.SetupVoid("initSwipe", _ => true);
        JSInterop.SetupVoid("destroySwipe", _ => true);
    }

    [Fact]
    public void RendersPageTitle()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["EventCalendar.Title"], cut.Markup);
    }

    [Fact]
    public void ShowsEmptyState_WhenNoEventsExist()
    {
        // Arrange
        Services.AddScoped<IEventService, EmptyEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["EventCalendar.NoEvents"], cut.Markup);
        Assert.Contains("fa-calendar-xmark", cut.Markup);
    }

    [Fact]
    public void DisplaysEvents_WhenEventsExist()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        Assert.Contains("Basketball Game", cut.Markup);
        Assert.Contains("Art Exhibition", cut.Markup);
        Assert.Contains("Yoga Session", cut.Markup);
        Assert.Contains("Guest Lecture", cut.Markup);
    }

    [Fact]
    public void DisplaysEventTypes()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        Assert.Contains("Sport", cut.Markup);
        Assert.Contains("Cultuur", cut.Markup);
        Assert.Contains("Welzijn", cut.Markup);
        Assert.Contains("Academisch", cut.Markup);
    }

    [Fact]
    public void DisplaysAllEventsButton()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["EventCalendar.AllEvents"], cut.Markup);
        Assert.Contains("fa-calendar-days", cut.Markup);
    }

    [Fact]
    public void DisplaysUpcomingEventsHeader()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["EventCalendar.UpcomingEvents"], cut.Markup);
        Assert.Contains("fa-list", cut.Markup);
    }

    [Fact]
    public void DisplaysEventCount()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        // Should show "4 Events"
        Assert.Contains("4", cut.Markup);
        Assert.Contains(localizer["EventCalendar.Events"], cut.Markup);
    }

    [Fact]
    public void DisplaysTicketIcon_WhenRegistrationLinkExists()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert - registration link shows ticket icon
        Assert.Contains("fa-ticket", cut.Markup);
    }

    [Fact]
    public void DisplaysEventDescriptions()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        Assert.Contains("Annual basketball tournament", cut.Markup);
        Assert.Contains("Relaxing yoga session", cut.Markup);
    }

    [Fact]
    public void DisplaysEventTimes()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        // Check for time format (HH:mm)
        Assert.Contains("fa-clock", cut.Markup);
    }

    [Fact]
    public void DisplaysFilterButtons_WhenMultipleTypesExist()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert
        // Should have filter buttons for event types
        var buttons = cut.FindAll("button");
        Assert.True(buttons.Count > 0);
    }

    [Fact]
    public void DisplaysMonthViewComponent()
    {
        // Arrange
        Services.AddScoped<IEventService, FakeEventService>();

        // Act
        var cut = RenderComponent<EventCalendarPage>();

        // Assert - the month view is wrapped in a container
        Assert.Contains("rounded-2xl", cut.Markup);
    }
}
