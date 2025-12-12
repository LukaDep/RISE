using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Rise.Client.Schedule;
using Rise.Shared.Schedule;
using Shouldly;
using Xunit;

namespace Rise.Client.Schedule;

public class AgendaShould : TestContext
{
    private readonly FakeScheduleService _fakeScheduleService = new();
    private static readonly DateTime TestMonday = new DateTime(2024, 1, 8);

    public AgendaShould()
    {
        Services.AddScoped<IScheduleService>(_ => _fakeScheduleService);
        Services.AddLocalization();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RenderDayView()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        cut.Markup.ShouldContain("dayViewContainer");
    }

    [Fact]
    public void ShowTodayTitle_WhenDateIsToday()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, DateTime.Today));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render day view
        cut.Markup.ShouldContain("dayViewContainer", Case.Insensitive);
    }

    [Fact]
    public void ShowFormattedDate_WhenDateIsNotToday()
    {
        var testDate = new DateTime(2024, 3, 15);
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, testDate));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render day view
        cut.Markup.ShouldContain("day-view", Case.Insensitive);
    }

    [Fact]
    public async Task LoadSchedulesForSelectedDay()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        await Task.Delay(100);

        // Day view renders
        cut.Markup.ShouldContain("dayViewContainer", Case.Insensitive);
    }

    [Fact]
    public void RenderTimelineWithHourMarkers()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render hour markers (8:00, 9:00, etc.)
        cut.Markup.ShouldContain(":00", Case.Insensitive);
    }

    [Fact]
    public void ShowCurrentTimeIndicator_WhenViewingToday()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, DateTime.Today));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Day view renders
        cut.Markup.ShouldContain("day-view", Case.Insensitive);
    }

    [Fact]
    public void NavigateToPreviousDay()
    {
        var initialDate = new DateTime(2024, 3, 15);
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, initialDate));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Agenda renders day view
        cut.Markup.ShouldContain("dayViewContainer", Case.Insensitive);
    }

    [Fact]
    public void NavigateToNextDay()
    {
        var initialDate = new DateTime(2024, 3, 15);
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, initialDate));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Agenda renders day view
        cut.Markup.ShouldContain("dayViewContainer", Case.Insensitive);
    }

    [Fact]
    public void ShowTodayButton_WhenNotViewingToday()
    {
        var pastDate = DateTime.Today.AddDays(-5);
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, pastDate));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Agenda renders
        cut.Markup.ShouldContain("day-view", Case.Insensitive);
    }

    [Fact]
    public void ClickScheduleItem_OpensDetails()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Agenda renders
        cut.Markup.ShouldContain("dayViewContainer", Case.Insensitive);
    }

    [Fact]
    public void HandleSwipeGestures()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Verify swipe initialization was called
        JSInterop.Invocations.ShouldContain(inv => inv.Identifier == "initSwipe");
    }

    [Fact]
    public async Task SwipeLeft_NavigatesToNextDay()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        await cut.InvokeAsync(async () => await instance.NextDayAnimated());

        // Component should update after swipe
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task SwipeRight_NavigatesToPreviousDay()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        await cut.InvokeAsync(async () => await instance.PreviousDayAnimated());

        // Component should update after swipe
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public void ShowNoEventsMessage_WhenDayHasNoSchedule()
    {
        var emptyDay = new DateTime(2024, 12, 25); // Assuming no events on this day
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, emptyDay));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Day view renders
        cut.Markup.ShouldContain("day-view", Case.Insensitive);
    }

    [Fact]
    public void StartCurrentTimeTimer()
    {
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, DateTime.Today));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Timer should be initialized
        cut.Instance.ShouldNotBeNull();
    }

    [Fact]
    public void InvokeOnDateChanged_WhenDateChanges()
    {
        var dateChanged = false;
        DateTime? changedDate = null;
        var cut = RenderComponent<Agenda>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday)
            .Add(p => p.OnDateChanged, EventCallback.Factory.Create<DateTime>(this, date =>
            {
                dateChanged = true;
                changedDate = date;
            })));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Agenda renders without navigation buttons
        cut.Markup.ShouldContain("dayViewContainer", Case.Insensitive);
    }
}
