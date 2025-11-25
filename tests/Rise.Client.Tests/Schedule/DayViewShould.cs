using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Rise.Shared.Schedule;
using Rise.Shared.Common;

namespace Rise.Client.Schedule;

public class DayViewShould : TestContext
{
    public DayViewShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IScheduleService, FakeScheduleService>();
        JSInterop.SetupVoid("initSwipe", _ => true);
    }

    [Fact]
    public void ShowsScheduleItemForToday()
    {
        // Arrange
        var cut = RenderComponent<DayView>(parameters => parameters.Add(p => p.SelectedDate, DateTime.Today));

        // Act
        var markup = cut.Markup;

        // Assert that the fake schedule item's course for today is visible
        Assert.Contains("Web Ontwikkeling 2", markup);
        Assert.Contains("GSCHB.2.009", markup);
    }

    [Fact]
    public void NavigateToPreviousDay_SkippingWeekend()
    {
        // Start on a Monday
        var monday = new DateTime(2024, 1, 8);
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, monday));

        cut.InvokeAsync(() => cut.Instance.PreviousDay());

        // Should skip Saturday and Sunday, landing on Friday
        var expectedDate = new DateTime(2024, 1, 5);
        Assert.Equal(expectedDate, cut.Instance.SelectedDate);
    }

    [Fact]
    public void NavigateToNextDay_SkippingWeekend()
    {
        // Start on a Friday
        var friday = new DateTime(2024, 1, 5);
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, friday));

        cut.InvokeAsync(() => cut.Instance.NextDay());

        // Should skip Saturday and Sunday, landing on Monday
        var expectedDate = new DateTime(2024, 1, 8);
        Assert.Equal(expectedDate, cut.Instance.SelectedDate);
    }

    [Fact]
    public void NavigateThroughMultipleWeekdays()
    {
        var tuesday = new DateTime(2024, 1, 9);
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, tuesday));

        // Navigate forward 3 days
        cut.InvokeAsync(() =>
        {
            cut.Instance.NextDay();
            cut.Instance.NextDay();
            cut.Instance.NextDay();
        });

        Assert.Equal(new DateTime(2024, 1, 12), cut.Instance.SelectedDate);
        Assert.Equal(DayOfWeek.Friday, cut.Instance.SelectedDate.DayOfWeek);
    }

    [Fact]
    public void GoToToday_FromPastDate()
    {
        var pastDate = DateTime.Today.AddDays(-10);
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, pastDate));

        cut.InvokeAsync(() => cut.Instance.GoToToday());

        Assert.Equal(DateTime.Today, cut.Instance.SelectedDate);
    }

    [Fact]
    public void GoToToday_FromFutureDate()
    {
        var futureDate = DateTime.Today.AddDays(10);
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, futureDate));

        cut.InvokeAsync(() => cut.Instance.GoToToday());

        Assert.Equal(DateTime.Today, cut.Instance.SelectedDate);
    }

    [Fact]
    public async Task OpenDetailsWhenScheduleItemClicked()
    {
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, DateTime.Today));

        var service = Services.GetRequiredService<IScheduleService>();
        var result = await service.GetIndexAsync(new QueryRequest.SkipTake());
        var schedules = result.Value!.Schedules;
        var firstSchedule = schedules!.First(s => s.StartDateTime.Date == DateTime.Today);

        await cut.InvokeAsync(() => cut.Instance.OpenDetails(firstSchedule));

        // Verify modal is rendered in markup
        Assert.Contains(firstSchedule.Course, cut.Markup);
    }

    [Fact]
    public async Task CloseDetailsWhenClosed()
    {
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, DateTime.Today));

        var service = Services.GetRequiredService<IScheduleService>();
        var result = await service.GetIndexAsync(new QueryRequest.SkipTake());
        var schedules = result.Value!.Schedules;
        var firstSchedule = schedules!.First(s => s.StartDateTime.Date == DateTime.Today);

        await cut.InvokeAsync(() => cut.Instance.OpenDetails(firstSchedule));

        // Check if modal is opened - SelectedSchedule should be set
        var selectedScheduleField = typeof(DayView).GetField("SelectedSchedule", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var selectedSchedule = selectedScheduleField?.GetValue(cut.Instance);
        Assert.NotNull(selectedSchedule);

        await cut.InvokeAsync(() => cut.Instance.CloseDetails());

        // Check if modal is closed - SelectedSchedule should be null
        selectedSchedule = selectedScheduleField?.GetValue(cut.Instance);
        Assert.Null(selectedSchedule);
    }

    [Fact]
    public void FilterSchedulesToSelectedDate()
    {
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, DateTime.Today));

        var daySchedules = cut.Instance.DaySchedules;
        Assert.All(daySchedules, schedule =>
            Assert.Equal(DateTime.Today.Date, schedule.StartDateTime.Date));
    }

    [Fact]
    public void ReturnEmptyListWhenNoSchedulesForDate()
    {
        var dateWithoutSchedules = DateTime.Today.AddDays(10);
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, dateWithoutSchedules));

        Assert.Empty(cut.Instance.DaySchedules);
    }

    [Fact]
    public void TruncateTitle_WhenTooLong()
    {
        var longTitle = "This is a very long course title that should be truncated";
        var truncated = DayView.TruncateTitle(longTitle, 20);

        Assert.Equal("This is a very long ...", truncated);
        Assert.True(truncated.Length <= 23);
    }

    [Fact]
    public void NotTruncateTitle_WhenShortEnough()
    {
        var shortTitle = "Short Title";
        var result = DayView.TruncateTitle(shortTitle, 20);

        Assert.Equal(shortTitle, result);
    }

    [Fact]
    public void GetCorrectBackgroundColor_ForWorkFormType()
    {
        Assert.Equal("bg-hogent-education-30 text-hogent-education",
            DayView.GetEventTypeBgColor("Hoorcollege"));
        Assert.Equal("bg-hogent-it-30 text-hogent-it",
            DayView.GetEventTypeBgColor("Activerend hoorcollege"));
        Assert.Equal("bg-hogent-green-30 text-hogent-green",
            DayView.GetEventTypeBgColor("Practicum"));
        Assert.Equal("bg-hogent-orange-30 text-hogent-orange",
            DayView.GetEventTypeBgColor("Werkcollege"));
        Assert.Equal("bg-hogent-business-30 text-hogent-business",
            DayView.GetEventTypeBgColor("Seminarie"));
        Assert.Equal("bg-hogent-black-30 text-hogent-black",
            DayView.GetEventTypeBgColor("Unknown Type"));
    }

    [Fact]
    public async Task SwipeNext_CallsNextDayAnimated()
    {
        var friday = new DateTime(2024, 1, 5);
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, friday));

        await cut.InvokeAsync(() => cut.Instance.SwipeNext());

        // Should skip to Monday
        Assert.Equal(new DateTime(2024, 1, 8), cut.Instance.SelectedDate);
    }

    [Fact]
    public async Task SwipePrevious_CallsPreviousDayAnimated()
    {
        var monday = new DateTime(2024, 1, 8);
        var cut = RenderComponent<DayView>(parameters => parameters
            .Add(p => p.SelectedDate, monday));

        await cut.InvokeAsync(() => cut.Instance.SwipePrevious());

        // Should skip to Friday
        Assert.Equal(new DateTime(2024, 1, 5), cut.Instance.SelectedDate);
    }
}
