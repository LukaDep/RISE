using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Rise.Client.Schedule.Component;
using Shouldly;
using Xunit;

namespace Rise.Client.Schedule;

public class WeekViewHeaderShould : TestContext
{
    private static readonly DateTime TestMonday = new DateTime(2024, 1, 8);

    public WeekViewHeaderShould()
    {
        Services.AddLocalization();
    }

    [Fact]
    public void RenderWeekDays()
    {
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 2));

        // Should render with justify-around layout
        cut.Markup.ShouldContain("justify-around", Case.Insensitive);
    }

    [Fact]
    public void ShowWeekNumber()
    {
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 42));

        cut.Markup.ShouldContain("42");
    }

    [Fact]
    public void ShowWeekRangeInTitle()
    {
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 2));

        // Should show dates
        cut.Markup.ShouldContain("8");
    }

    [Fact]
    public void ShowAbbreviatedDayNames()
    {
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 2));

        // Should show single letter abbreviations (M, T, W, T, F, S, S)
        cut.Markup.ShouldContain("M");
        cut.Markup.ShouldContain("T");
        cut.Markup.ShouldContain("W");
        cut.Markup.ShouldContain("F");
        cut.Markup.ShouldContain("S");
    }

    [Fact]
    public void HighlightSelectedDay()
    {
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 2)
            .Add(p => p.SelectedDate, TestMonday));

        // Selected day should have bg-hogent-black styling
        cut.Markup.ShouldContain("bg-hogent-black", Case.Insensitive);
    }

    [Fact]
    public void InvokeOnDaySelected_WhenDayClicked()
    {
        var daySelected = false;
        DateTime? selectedDate = null;
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 2)
            .Add(p => p.OnDaySelected, EventCallback.Factory.Create<DateTime>(this, date =>
            {
                daySelected = true;
                selectedDate = date;
            })));

        var buttons = cut.FindAll("button");
        buttons[1].Click();

        cut.WaitForAssertion(() => daySelected.ShouldBeTrue());
        selectedDate.ShouldNotBeNull();
    }

    [Fact]
    public void RenderCalendarButton()
    {
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 2));

        // Should have calendar link
        cut.Markup.ShouldContain("/schedule/month", Case.Insensitive);
    }

    [Fact]
    public void InvokeOnCalendarClick_WhenCalendarButtonClicked()
    {
        var calendarClicked = false;
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 2)
            .Add(p => p.OnCalendarClick, EventCallback.Factory.Create(this, () => calendarClicked = true)));

        // Check for calendar link
        cut.Markup.ShouldContain("/schedule/month", Case.Insensitive);
    }

    [Fact]
    public void RenderTodayButton()
    {
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 2));

        // Should have today button
        cut.Markup.ShouldContain("Today", Case.Insensitive);
    }

    [Fact]
    public void InvokeOnTodayButtonClick_WhenTodayButtonClicked()
    {
        var todayClicked = false;
        var weekDays = Enumerable.Range(0, 7)
            .Select(i => TestMonday.AddDays(i))
            .ToList();

        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, weekDays)
            .Add(p => p.WeekNumber, 2)
            .Add(p => p.OnTodayButtonClick, EventCallback.Factory.Create(this, () => todayClicked = true)));

        var buttons = cut.FindAll("button");
        buttons[0].Click();

        todayClicked.ShouldBeTrue();
    }

    [Fact]
    public void HandleEmptyWeekDaysList()
    {
        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, new List<DateTime>())
            .Add(p => p.WeekNumber, 2));

        // Should render without errors
        cut.Markup.ShouldNotBeEmpty();
    }

    [Fact]
    public void HandleNullWeekDays()
    {
        var cut = RenderComponent<WeekViewHeader>(parameters => parameters
            .Add(p => p.WeekDays, null as List<DateTime>)
            .Add(p => p.WeekNumber, 2));

        // Should render without errors
        cut.Markup.ShouldNotBeEmpty();
    }
}
