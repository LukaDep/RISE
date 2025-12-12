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
    public void RendersScheduleComponent()
    {
        // Arrange & Act
        var cut = RenderComponent<Index>();

        // Assert - Schedule component renders with agenda view (dayViewContainer)
        Assert.Contains("dayViewContainer", cut.Markup);
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
}
