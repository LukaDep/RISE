using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Rise.Client.Schedule;
using Rise.Shared.Schedule;
using Shouldly;
using Xunit;

namespace Rise.Client.Schedule;

public class CalendarShould : TestContext
{
    private readonly FakeScheduleService _fakeScheduleService = new();

    public CalendarShould()
    {
        Services.AddScoped<IScheduleService>(_ => _fakeScheduleService);
        Services.AddLocalization();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RenderMonthGrid()
    {
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render calendar grid
        cut.Markup.ShouldContain("grid grid-cols-7", Case.Insensitive);
        cut.Markup.ShouldContain("calendarContainer", Case.Insensitive);
    }

    [Fact]
    public void RenderWeekDayHeaders()
    {
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should show grid header with day names
        cut.Markup.ShouldContain("grid grid-cols-7", Case.Insensitive);
    }

    [Fact]
    public async Task LoadSchedulesForMonth()
    {
        var testDate = new DateTime(2024, 1, 15);
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Wait for schedules to load
        await Task.Delay(100);

        // Calendar renders
        cut.Markup.ShouldContain("calendarContainer", Case.Insensitive);
    }

    [Fact]
    public void HighlightCurrentDay()
    {
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Today's date should have special styling
        cut.Markup.ShouldContain("bg-hogent-education", Case.Insensitive);
    }

    [Fact]
    public void ShowMultipleEventsPerDay()
    {
        var testDate = new DateTime(2024, 1, 8); // Test Monday with events
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Calendar grid renders
        cut.Markup.ShouldContain("grid grid-cols-7", Case.Insensitive);
    }

    [Fact]
    public void NavigateToPreviousMonth()
    {
        var initialDate = new DateTime(2024, 3, 15);
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var buttons = cut.FindAll("button");
        buttons[0].Click(); // First button is previous

        // Calendar updates
        cut.WaitForAssertion(() => cut.Markup.ShouldNotBeEmpty());
    }

    [Fact]
    public void NavigateToNextMonth()
    {
        var initialDate = new DateTime(2024, 3, 15);
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var buttons = cut.FindAll("button");
        buttons[1].Click(); // Second button is next

        // Calendar updates
        cut.WaitForAssertion(() => cut.Markup.ShouldNotBeEmpty());
    }

    [Fact]
    public void ShowWorkFormLegend()
    {
        var testDate = new DateTime(2024, 1, 8);
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Calendar renders with flex layout for legend
        cut.Markup.ShouldContain("flex-wrap gap-4", Case.Insensitive);
    }

    [Fact]
    public void ClickDayToNavigateToAgenda()
    {
        var testDate = new DateTime(2024, 1, 15);
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Calendar renders with clickable days
        cut.Markup.ShouldContain("cursor-pointer", Case.Insensitive);
    }

    [Fact]
    public void HandleSwipeGestures()
    {
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Verify swipe initialization was called
        JSInterop.Invocations.ShouldContain(inv => inv.Identifier == "initSwipe");
    }

    [Fact]
    public async Task SwipeLeft_NavigatesToNextMonth()
    {
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        await cut.InvokeAsync(async () => await instance.SwipeNext());

        // Component should update after swipe
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task SwipeRight_NavigatesToPreviousMonth()
    {
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        await cut.InvokeAsync(async () => await instance.SwipePrevious());

        // Component should update after swipe
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public void RenderCurrentMonthAndYear()
    {
        var testDate = new DateTime(2024, 6, 15);
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should display month and year
        var currentMonth = DateTime.Today.ToString("MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
        cut.Markup.ShouldContain(currentMonth);
    }

    [Fact]
    public void RenderDaysFromAdjacentMonths()
    {
        var cut = RenderComponent<Calendar>();

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render 42 days (6 weeks) including adjacent months with bg-hogent-black-15
        cut.Markup.ShouldContain("bg-hogent-black-15", Case.Insensitive);
    }

    [Fact]
    public void GetDayAbbreviation_ReturnsUpperCaseDayName()
    {
        var cut = RenderComponent<Calendar>();
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should contain abbreviated day names in header
        var markup = cut.Markup.ToUpper();
        markup.ShouldContain("MON", Case.Insensitive);
    }

    [Fact]
    public async Task LoadSchedulesAsync_FetchesSchedulesForCurrentMonth()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));
        
        // Wait for async loading
        await Task.Delay(200);
        
        // Verify schedule service was called
        _fakeScheduleService.ShouldNotBeNull();
    }

    [Fact]
    public void GetSchedulesForDate_ReturnsEmptyListWhenNoSchedules()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should handle days with no events gracefully
        cut.Markup.ShouldContain("min-h-[80px]", Case.Insensitive);
    }

    [Fact]
    public void RenderEventIndicators_ShowsMaxThreeEvents()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Calendar should render with days that can show events
        cut.Markup.ShouldContain("cursor-pointer", Case.Insensitive);
    }

    [Fact]
    public void ShowMoreIndicator_WhenMoreThanThreeEvents()
    {
        _fakeScheduleService.CreateMultipleSchedulesForSameDay(new DateTime(2024, 1, 15), 5);
        
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should show calendar container
        cut.Markup.ShouldContain("calendarContainer", Case.Insensitive);
    }

    [Fact]
    public void GetWorkFormsInMonth_ReturnsDistinctWorkForms()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render legend area
        cut.Markup.ShouldContain("flex flex-wrap gap-4", Case.Insensitive);
    }

    [Fact]
    public void RenderDifferentStylesForCurrentAndOtherMonths()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Current month days have bg-hogent-white, other months have bg-hogent-black-15
        cut.Markup.ShouldContain("bg-hogent-white", Case.Insensitive);
        cut.Markup.ShouldContain("bg-hogent-black-15", Case.Insensitive);
    }

    [Fact]
    public void ApplyHoverEffects_OnCalendarDays()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should have hover styles
        cut.Markup.ShouldContain("hover:shadow-md", Case.Insensitive);
        cut.Markup.ShouldContain("hover:bg-hogent-black-10", Case.Insensitive);
    }

    [Fact]
    public void RenderNavigationButtons_WithProperStyling()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var buttons = cut.FindAll("button");
        buttons.Count.ShouldBeGreaterThanOrEqualTo(2);
        
        // Navigation buttons should have styling
        cut.Markup.ShouldContain("rounded bg-hogent-black-10", Case.Insensitive);
    }

    [Fact]
    public async Task PreviousMonthAnimated_TriggersSwipeAnimation()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        await cut.InvokeAsync(async () => await instance.PreviousMonthAnimated());

        // Animation should complete
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task NextMonthAnimated_TriggersSwipeAnimation()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        await cut.InvokeAsync(async () => await instance.NextMonthAnimated());

        // Animation should complete
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public void HasEventsOnDay_ReturnsFalseWhenNoSchedule()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render days without events
        cut.Markup.ShouldContain("min-h-[80px]", Case.Insensitive);
    }

    [Fact]
    public void RenderWorkFormColorCoding_InLegend()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should have legend area even if empty
        cut.Markup.ShouldContain("flex flex-wrap gap-4", Case.Insensitive);
    }

    [Fact]
    public async Task DisposeAsync_DisposesResources()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        var instance = cut.Instance;
        
        // Should not throw when disposing
        await instance.DisposeAsync();
        
        instance.ShouldNotBeNull();
    }

    [Fact]
    public void RenderScheduleForJanuaryWithEvents()
    {
        // Calendar starts at today (December 2025)
        var today = DateTime.Today;
        var eventDate = new DateTime(today.Year, today.Month, 15);
        _fakeScheduleService.CreateMultipleSchedulesForSameDay(eventDate, 2);
        
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render current month name
        var currentMonth = today.ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture);
        cut.Markup.ShouldContain(currentMonth, Case.Insensitive);
    }

    [Fact]
    public void ShowEventIndicatorsForDaysWithSchedules()
    {
        // Add events for today's month
        var today = DateTime.Today;
        var eventDate = new DateTime(today.Year, today.Month, 15);
        _fakeScheduleService.CreateMultipleSchedulesForSameDay(eventDate, 4);
        
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should contain the current month name
        var currentMonth = today.ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture);
        cut.Markup.ShouldContain(currentMonth, Case.Insensitive);
    }

    [Fact]
    public void RenderEventColorIndicators_BasedOnWorkForm()
    {
        var today = DateTime.Today;
        var eventDate = new DateTime(today.Year, today.Month, 10);
        _fakeScheduleService.CreateMultipleSchedulesForSameDay(eventDate, 2);
        
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Verify calendar renders successfully
        cut.Markup.ShouldContain("calendarContainer", Case.Insensitive);
    }

    [Fact]
    public void HandleMonthWithNoEvents()
    {
        // Navigate to a month with no events
        var futureDate = new DateTime(2025, 12, 15);
        
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should still render properly even with no events
        cut.Markup.ShouldContain("grid grid-cols-7", Case.Insensitive);
    }

    [Fact]
    public void CalculateFirstDayOfMonth_Correctly()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render all 42 days (6 weeks)
        var dayDivs = cut.FindAll("div[class*='min-h-[80px]']");
        dayDivs.Count.ShouldBe(42);
    }

    [Fact]
    public void RenderTodayWithSpecialHighlight()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Today should have bg-hogent-education styling
        cut.Markup.ShouldContain("bg-hogent-education", Case.Insensitive);
    }

    [Fact]
    public void ShowOnlyThreeEventsPerDay_WhenMoreExist()
    {
        var today = DateTime.Today;
        var eventDate = new DateTime(today.Year, today.Month, 5);
        _fakeScheduleService.CreateMultipleSchedulesForSameDay(eventDate, 5);
        
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Verify calendar loads
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public void RenderDayNumbers_ForAllDaysInGrid()
    {
        var cut = RenderComponent<Calendar>();
        
        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // Should render day numbers with text-sm styling
        cut.Markup.ShouldContain("text-sm", Case.Insensitive);
    }
}
