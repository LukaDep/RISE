using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Rise.Shared.Schedule;
using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule;

public class WeekViewShould : TestContext
{
    public WeekViewShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IScheduleService, FakeScheduleService>();
        JSInterop.SetupVoid("initSwipe", _ => true);
    }

    [Fact]
    public void RenderHeaderRangeAndEvents()
    {
        var today = DateTime.Today;
        var dayOfWeek = (int)today.DayOfWeek;
        var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1; // Monday=1
        var weekStart = today.AddDays(-daysToSubtract);
        var expectedStart = weekStart.ToString("d MMM", CultureInfo.CurrentCulture);
        var expectedEnd = weekStart.AddDays(6).ToString("d MMM", CultureInfo.CurrentCulture);
        var cut = RenderComponent<WeekView>(p => p.Add(x => x.SelectedDate, today));
        var markup = cut.Markup;
        Assert.Contains(expectedStart, markup);
        Assert.Contains(expectedEnd, markup);
        Assert.Contains("Web Ontwikkeling 2", markup);
    }

    [Fact]
    public async Task NavigateToPreviousWeek()
    {
        var monday = new DateTime(2024, 1, 8); // Week 2
        var cut = RenderComponent<WeekView>(p => p.Add(x => x.SelectedDate, monday));

        await cut.InvokeAsync(() => cut.Instance.SwipePrevious());
        var expectedDate = new DateTime(2024, 1, 1);
        Assert.Equal(expectedDate, cut.Instance.SelectedDate);
    }

    [Fact]
    public async Task NavigateToNextWeek()
    {
        var monday = new DateTime(2024, 1, 8); // Week 2
        var cut = RenderComponent<WeekView>(p => p.Add(x => x.SelectedDate, monday));

        await cut.InvokeAsync(() => cut.Instance.SwipeNext());
        var expectedDate = new DateTime(2024, 1, 15);
        Assert.Equal(expectedDate, cut.Instance.SelectedDate);
    }

    [Fact]
    public async Task GoToToday_FromPastDate()
    {
        var pastDate = DateTime.Today.AddDays(-30);
        var cut = RenderComponent<WeekView>(p => p.Add(x => x.SelectedDate, pastDate));
        var goToTodayMethod = typeof(WeekView).GetMethod("GoToToday", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await cut.InvokeAsync(async () => await (Task)goToTodayMethod!.Invoke(cut.Instance, null)!);

        Assert.Equal(DateTime.Today, cut.Instance.SelectedDate);
    }

    [Fact]
    public async Task GoToToday_FromFutureDate()
    {
        var futureDate = DateTime.Today.AddDays(30);
        var cut = RenderComponent<WeekView>(p => p.Add(x => x.SelectedDate, futureDate));
        var goToTodayMethod = typeof(WeekView).GetMethod("GoToToday", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await cut.InvokeAsync(async () => await (Task)goToTodayMethod!.Invoke(cut.Instance, null)!);

        Assert.Equal(DateTime.Today, cut.Instance.SelectedDate);
    }

}
