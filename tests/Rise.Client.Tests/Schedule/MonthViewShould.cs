using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Rise.Shared.Schedule;
using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule;

public class MonthViewShould : TestContext
{
    public MonthViewShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IScheduleService, FakeScheduleService>();
        JSInterop.SetupVoid("initSwipe", _ => true);
    }

    [Fact]
    public void RenderHeaderAndEventIndicators()
    {
        // Arrange
        var today = DateTime.Today;
        var expectedHeader = today.ToString("MMMM yyyy", CultureInfo.CurrentCulture);

        // Act
        var cut = RenderComponent<MonthView>(p => p.Add(x => x.SelectedDate, today));
        var markup = cut.Markup;

        // Assert header shows current month and year
        Assert.Contains(expectedHeader, markup);

        // Assert that event indicators (colored bars) appear for days with events
        Assert.Contains("w-full h-1", markup);
    }

    [Fact]
    public async Task NavigateToPreviousMonth()
    {
        var february = new DateTime(2024, 2, 15);
        var cut = RenderComponent<MonthView>(p => p.Add(x => x.SelectedDate, february));

        await cut.InvokeAsync(() => cut.Instance.SwipePrevious());

        Assert.Equal(new DateTime(2024, 1, 15), cut.Instance.SelectedDate);
    }

    [Fact]
    public async Task GoToToday_FromPastDate()
    {
        var pastDate = DateTime.Today.AddMonths(-3);
        var cut = RenderComponent<MonthView>(p => p.Add(x => x.SelectedDate, pastDate));

        // Use reflection to call private GoToToday method
        var goToTodayMethod = typeof(MonthView).GetMethod("GoToToday", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await cut.InvokeAsync(async () => await (Task)goToTodayMethod!.Invoke(cut.Instance, null)!);

        Assert.Equal(DateTime.Today, cut.Instance.SelectedDate);
    }

    [Fact]
    public async Task GoToToday_FromFutureDate()
    {
        var futureDate = DateTime.Today.AddMonths(3);
        var cut = RenderComponent<MonthView>(p => p.Add(x => x.SelectedDate, futureDate));

        // Use reflection to call private GoToToday method
        var goToTodayMethod = typeof(MonthView).GetMethod("GoToToday", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await cut.InvokeAsync(async () => await (Task)goToTodayMethod!.Invoke(cut.Instance, null)!);

        Assert.Equal(DateTime.Today, cut.Instance.SelectedDate);
    }
}
