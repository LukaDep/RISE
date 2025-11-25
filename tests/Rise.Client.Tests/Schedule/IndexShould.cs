using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using Rise.Shared.Schedule;
using NSubstitute;
using Rise.Shared.Campus;

namespace Rise.Client.Schedule;

public class IndexShould : TestContext
{
    public IndexShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IScheduleService, FakeScheduleService>();
        Services.AddScoped(_ => Substitute.For<ICampusService>());
        JSInterop.SetupVoid("initSwipe", _ => true);
    }

    [Fact]
    public void RendersHeaderAndTabs()
    {
        // Arrange & Act
        var cut = RenderComponent<Index>();

        // Assert header/title rendered from localizer
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains("<h2", cut.Markup);
        Assert.Contains(localizer["Schedule.Title"], cut.Markup);

        // Tabs should exist for Day/Week/Month
        Assert.Contains(localizer["Schedule.Day"], cut.Markup);
        Assert.Contains(localizer["Schedule.Week"], cut.Markup);
        Assert.Contains(localizer["Schedule.Month"], cut.Markup);
    }

    [Fact]
    public void InitializeWithTodayAsSelectedDay()
    {
        var cut = RenderComponent<Index>();

        var instance = cut.Instance;
        var selectedDayField = instance.GetType()
            .GetField("SelectedDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var selectedDay = (DateTime)selectedDayField!.GetValue(instance)!;

        Assert.Equal(DateTime.Today, selectedDay);
    }

    [Fact]
    public void GoToDay_UpdatesSelectedDay()
    {
        var cut = RenderComponent<Index>();
        var instance = cut.Instance;

        var goToDayMethod = instance.GetType()
            .GetMethod("GoToDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var newDate = DateTime.Today.AddDays(5);
        cut.InvokeAsync(() => goToDayMethod!.Invoke(instance, new object[] { newDate }));

        var selectedDayField = instance.GetType()
            .GetField("SelectedDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var selectedDay = (DateTime)selectedDayField!.GetValue(instance)!;

        Assert.Equal(newDate, selectedDay);
    }

    [Fact]
    public void GoToDay_ActivatesDayTab()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        // Click on Month tab first
        var monthTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Month"]));
        monthTabButton.Click();

        // Verify Month tab is active
        var activeButtons = cut.FindAll("button.bg-hogent-white");
        Assert.Contains(activeButtons, b => b.TextContent.Contains(localizer["Schedule.Month"]));

        // Invoke GoToDay through reflection
        var instance = cut.Instance;
        var goToDayMethod = instance.GetType()
            .GetMethod("GoToDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var newDate = DateTime.Today.AddDays(3);
        cut.InvokeAsync(() => goToDayMethod!.Invoke(instance, new object[] { newDate }));

        // Re-render and check
        activeButtons = cut.FindAll("button.bg-hogent-white");

        // Day tab should now be active
        Assert.Contains(activeButtons, b => b.TextContent.Contains(localizer["Schedule.Day"]));
    }

    [Fact]
    public void DayViewReceivesSelectedDay()
    {
        var cut = RenderComponent<Index>();

        // Get SelectedDay value
        var instance = cut.Instance;
        var selectedDayField = instance.GetType()
            .GetField("SelectedDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var selectedDay = (DateTime)selectedDayField!.GetValue(instance)!;

        // DayView should be rendered with this date
        // We can verify through the markup that it contains today's content
        Assert.Equal(DateTime.Today, selectedDay);
        Assert.Contains("Web Ontwikkeling 2", cut.Markup); // From FakeScheduleService for today
    }

    [Fact]
    public void WeekViewReceivesSelectedDay()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        // Click Week tab
        var weekTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Week"]));
        weekTabButton.Click();

        // WeekView should render with today
        Assert.Contains("Web Ontwikkeling 2", cut.Markup);
    }

    [Fact]
    public void MonthViewReceivesSelectedDay()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        // Click Month tab
        var monthTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Month"]));
        monthTabButton.Click();

        // MonthView should render with current month
        var currentMonth = DateTime.Today.ToString("MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
        Assert.Contains(currentMonth, cut.Markup);
    }

    [Fact]
    public void TabSwitching_PreservesSelectedDay()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        // Start on Day tab, change date through GoToDay
        var instance = cut.Instance;
        var goToDayMethod = instance.GetType()
            .GetMethod("GoToDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var newDate = DateTime.Today.AddDays(5);
        cut.InvokeAsync(() => goToDayMethod!.Invoke(instance, new object[] { newDate }));

        // Switch to Week tab
        var weekTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Week"]));
        weekTabButton.Click();

        // Switch back to Day tab
        var dayTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Day"]));
        dayTabButton.Click();

        // SelectedDay should still be the updated date
        var selectedDayField = instance.GetType()
            .GetField("SelectedDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var selectedDay = (DateTime)selectedDayField!.GetValue(instance)!;

        Assert.Equal(newDate, selectedDay);
    }

    [Fact]
    public void AllThreeTabsAreClickable()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        // Click Day tab and verify
        cut.InvokeAsync(() =>
        {
            var dayTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Day"]));
            dayTab.Click();
        });
        var activeDayTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Day"]));
        Assert.Contains("bg-hogent-white", activeDayTab.GetAttribute("class"));

        // Click Week tab and verify
        cut.InvokeAsync(() =>
        {
            var weekTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Week"]));
            weekTab.Click();
        });
        var activeWeekTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Week"]));
        Assert.Contains("bg-hogent-white", activeWeekTab.GetAttribute("class"));

        // Click Month tab and verify
        cut.InvokeAsync(() =>
        {
            var monthTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Month"]));
            monthTab.Click();
        });
        var activeMonthTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Month"]));
        Assert.Contains("bg-hogent-white", activeMonthTab.GetAttribute("class"));
    }
}
