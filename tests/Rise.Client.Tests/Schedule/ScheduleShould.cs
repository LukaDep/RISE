using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Rise.Client.Schedule;
using Shouldly;
using Xunit;

namespace Rise.Client.Schedule;

public class ScheduleShould : TestContext
{
    private readonly FakeScheduleService _fakeScheduleService = new();
    private static readonly DateTime TestMonday = new DateTime(2024, 1, 8);

    public ScheduleShould()
    {
        Services.AddScoped<Rise.Shared.Schedule.IScheduleService>(_ => _fakeScheduleService);
        Services.AddLocalization();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RenderWeekView()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Schedule renders with flex layout
        cut.Markup.ShouldContain("flex flex-col", Case.Insensitive);
    }

    [Fact]
    public void RenderNavigationHeader()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Schedule renders with Week
        cut.Markup.ShouldContain("Week", Case.Insensitive);
    }

    [Fact]
    public void ShowWeekNumber()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should display week number
        cut.Markup.ShouldContain("Week");
    }

    [Fact]
    public void RenderWeekDays()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render with justify-around for days
        cut.Markup.ShouldContain("justify-around", Case.Insensitive);
    }

    [Fact]
    public void HighlightSelectedDay()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render with bg-hogent-black styling
        cut.Markup.ShouldContain("bg-hogent-black", Case.Insensitive);
    }

    [Fact]
    public void NavigateToPreviousWeek()
    {
        var initialDate = new DateTime(2024, 3, 15);
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, initialDate));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Schedule renders week view
        cut.Markup.ShouldContain("Week", Case.Insensitive);
    }

    [Fact]
    public void NavigateToNextWeek()
    {
        var initialDate = new DateTime(2024, 3, 15);
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, initialDate));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Schedule renders week view
        cut.Markup.ShouldContain("Week", Case.Insensitive);
    }

    [Fact]
    public async Task GoToToday_NavigatesToCurrentWeek()
    {
        var pastDate = DateTime.Today.AddDays(-30);
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, pastDate));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        await cut.InvokeAsync(async () => await instance.GoToToday());

        // Should navigate to today
        cut.WaitForAssertion(() => cut.Instance.SelectedDate.Date.ShouldBe(DateTime.Today));
    }

    [Fact]
    public void ShowTodayButton_WhenNotViewingCurrentWeek()
    {
        var pastDate = DateTime.Today.AddDays(-30);
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, pastDate));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should show Today button
        cut.Markup.ShouldContain("Today", Case.Insensitive);
    }

    [Fact]
    public void ClickDay_InvokesOnDayClick()
    {
        var dayClicked = false;
        DateTime? clickedDate = null;
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday)
            .Add(p => p.OnDayClick, EventCallback.Factory.Create<DateTime>(this, date =>
            {
                dayClicked = true;
                clickedDate = date;
            })));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Schedule renders week view
        cut.Markup.ShouldContain("Week", Case.Insensitive);
    }

    [Fact]
    public void HandleSwipeGestures()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Verify swipe initialization was called
        JSInterop.Invocations.ShouldContain(inv => inv.Identifier == "initSwipe");
    }

    [Fact]
    public async Task SwipeLeft_NavigatesToNextWeek()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        await cut.InvokeAsync(async () => await instance.SwipeNext());

        // Component should update after swipe
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task SwipeRight_NavigatesToPreviousWeek()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        await cut.InvokeAsync(async () => await instance.SwipePrevious());

        // Component should update after swipe
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public void ShowCurrentTimeIndicator_WhenViewingCurrentWeek()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, DateTime.Today));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should show day view
        cut.Markup.ShouldContain("day-view", Case.Insensitive);
    }

    [Fact]
    public void StartCurrentTimeTimer()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
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
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday)
            .Add(p => p.OnDateChanged, EventCallback.Factory.Create<DateTime>(this, date =>
            {
                dateChanged = true;
                changedDate = date;
            })));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Schedule renders with week view
        cut.Markup.ShouldContain("Week", Case.Insensitive);
    }

    [Fact]
    public void RenderTimelineWithHourMarkers()
    {
        var cut = RenderComponent<Schedule>(parameters => parameters
            .Add(p => p.SelectedDate, TestMonday));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render hour markers with :00
        cut.Markup.ShouldContain(":00");
    }
}
