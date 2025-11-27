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
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains("<h2", cut.Markup);
        Assert.Contains(localizer["Schedule.Title"], cut.Markup);
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
        var monthTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Month"]));
        monthTabButton.Click();
        var activeButtons = cut.FindAll("button.bg-hogent-white");
        Assert.Contains(activeButtons, b => b.TextContent.Contains(localizer["Schedule.Month"]));
        var instance = cut.Instance;
        var goToDayMethod = instance.GetType()
            .GetMethod("GoToDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var newDate = DateTime.Today.AddDays(3);
        cut.InvokeAsync(() => goToDayMethod!.Invoke(instance, new object[] { newDate }));
        activeButtons = cut.FindAll("button.bg-hogent-white");
        Assert.Contains(activeButtons, b => b.TextContent.Contains(localizer["Schedule.Day"]));
    }

    [Fact]
    public void DayViewReceivesSelectedDay()
    {
        var cut = RenderComponent<Index>();
        var instance = cut.Instance;
        var selectedDayField = instance.GetType()
            .GetField("SelectedDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var selectedDay = (DateTime)selectedDayField!.GetValue(instance)!;
        Assert.Equal(DateTime.Today, selectedDay);
        Assert.Contains("Web Ontwikkeling 2", cut.Markup); // From FakeScheduleService for today
    }

    [Fact]
    public void WeekViewReceivesSelectedDay()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var weekTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Week"]));
        weekTabButton.Click();
        Assert.Contains("Web Ontwikkeling 2", cut.Markup);
    }

    [Fact]
    public void MonthViewReceivesSelectedDay()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var monthTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Month"]));
        monthTabButton.Click();
        var currentMonth = DateTime.Today.ToString("MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
        Assert.Contains(currentMonth, cut.Markup);
    }

    [Fact]
    public void TabSwitching_PreservesSelectedDay()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var instance = cut.Instance;
        var goToDayMethod = instance.GetType()
            .GetMethod("GoToDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var newDate = DateTime.Today.AddDays(5);
        cut.InvokeAsync(() => goToDayMethod!.Invoke(instance, new object[] { newDate }));
        var weekTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Week"]));
        weekTabButton.Click();
        var dayTabButton = cut.FindAll("button")
            .First(b => b.TextContent.Contains(localizer["Schedule.Day"]));
        dayTabButton.Click();
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
        cut.InvokeAsync(() =>
        {
            var dayTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Day"]));
            dayTab.Click();
        });
        var activeDayTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Day"]));
        Assert.Contains("bg-hogent-white", activeDayTab.GetAttribute("class"));
        cut.InvokeAsync(() =>
        {
            var weekTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Week"]));
            weekTab.Click();
        });
        var activeWeekTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Week"]));
        Assert.Contains("bg-hogent-white", activeWeekTab.GetAttribute("class"));
        cut.InvokeAsync(() =>
        {
            var monthTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Month"]));
            monthTab.Click();
        });
        var activeMonthTab = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Month"]));
        Assert.Contains("bg-hogent-white", activeMonthTab.GetAttribute("class"));
    }
}
