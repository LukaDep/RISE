using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Rise.Shared.Campus;
using Rise.Shared.Schedule;
using Microsoft.AspNetCore.Components;
using Ardalis.Result;

namespace Rise.Client.Schedule;

public class ScheduleItemShould : TestContext
{
    private readonly ICampusService campusService;

    public ScheduleItemShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IScheduleService, FakeScheduleService>();

        campusService = Substitute.For<ICampusService>();
        Services.AddScoped(_ => campusService);

        JSInterop.SetupVoid("initSwipe", _ => true);
    }

    [Fact]
    public void OpenOnEventClickAndClose()
    {
        var cut = RenderComponent<DayView>(p => p.Add(x => x.SelectedDate, DateTime.Today));
        var eventDiv = cut.FindAll("div").First(e =>
            e.TextContent.Contains("Web Ontwikkeling 2") &&
            (e.GetAttribute("class")?.Contains("cursor-pointer") ?? false));
        eventDiv.Click();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains("Web Ontwikkeling 2", cut.Markup);
        Assert.Contains(localizer["Schedule.Close"], cut.Markup);
        var closeBtn = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Close"]));
        closeBtn.Click();
        Assert.DoesNotContain(localizer["Schedule.Close"], cut.Markup);
    }

    [Fact]
    public void DisplayAllScheduleProperties()
    {
        var schedule = new ScheduleDto.Schedule
        {
            Id = "test1",
            Course = "Advanced Programming",
            WorkForm = "Practicum",
            Environment = "Lab",
            Room = "GSCHB.4.101",
            Teacher = "John Doe",
            StartDateTime = new DateTime(2024, 2, 14, 10, 0, 0),
            EndDateTime = new DateTime(2024, 2, 14, 12, 0, 0),
            IsAbsent = false
        };

        bool closeCalled = false;
        var cut = RenderComponent<ScheduleItem>(parameters => parameters
            .Add(p => p.Schedule, schedule)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => closeCalled = true)));

        var markup = cut.Markup;

        Assert.Contains("Advanced Programming", markup);
        Assert.Contains("GSCHB.4.101", markup);
        Assert.Contains("John Doe", markup);
        Assert.Contains("10:00", markup);
        Assert.Contains("12:00", markup);
        Assert.Contains("Practicum", markup);
        Assert.Contains("Lab", markup);
    }

    [Fact]
    public void DisplayAbsentStatus_WhenIsAbsentTrue()
    {
        var schedule = new ScheduleDto.Schedule
        {
            Id = "test1",
            Course = "Advanced Programming",
            WorkForm = "Practicum",
            Environment = "Lab",
            Room = "GSCHB.4.101",
            Teacher = "John Doe",
            StartDateTime = new DateTime(2024, 2, 14, 10, 0, 0),
            EndDateTime = new DateTime(2024, 2, 14, 12, 0, 0),
            IsAbsent = true
        };

        var cut = RenderComponent<ScheduleItem>(parameters => parameters
            .Add(p => p.Schedule, schedule)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { })));

        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Schedule.Absent"], cut.Markup);
    }

    [Fact]
    public void HideAbsentStatus_WhenIsAbsentFalse()
    {
        var schedule = new ScheduleDto.Schedule
        {
            Id = "test1",
            Course = "Advanced Programming",
            IsAbsent = false,
            StartDateTime = DateTime.Today.AddHours(10),
            EndDateTime = DateTime.Today.AddHours(12)
        };

        var cut = RenderComponent<ScheduleItem>(parameters => parameters
            .Add(p => p.Schedule, schedule)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { })));

        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.DoesNotContain(localizer["Schedule.Absent"], cut.Markup);
    }

    [Fact]
    public async Task CloseDetails_InvokesOnCloseCallback()
    {
        var schedule = new ScheduleDto.Schedule
        {
            Id = "test1",
            Course = "Test Course",
            StartDateTime = DateTime.Today.AddHours(10),
            EndDateTime = DateTime.Today.AddHours(12)
        };

        bool closeCalled = false;
        var cut = RenderComponent<ScheduleItem>(parameters => parameters
            .Add(p => p.Schedule, schedule)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => closeCalled = true)));

        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var closeButton = cut.FindAll("button").First(b => b.TextContent.Contains(localizer["Schedule.Close"]));
        closeButton.Click();

        Assert.True(closeCalled);
    }

    [Fact]
    public void CloseDetails_WhenBackgroundClicked()
    {
        var schedule = new ScheduleDto.Schedule
        {
            Id = "test1",
            Course = "Test Course",
            StartDateTime = DateTime.Today.AddHours(10),
            EndDateTime = DateTime.Today.AddHours(12)
        };

        bool closeCalled = false;
        var cut = RenderComponent<ScheduleItem>(parameters => parameters
            .Add(p => p.Schedule, schedule)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => closeCalled = true)));
        var overlay = cut.Find("div.bg-black\\/50");
        overlay.Click();

        Assert.True(closeCalled);
    }

    [Fact]
    public async Task NavigateToRoom_CallsCampusService()
    {
        var schedule = new ScheduleDto.Schedule
        {
            Id = "test1",
            Course = "Test Course",
            Room = "GSCHB.4.101",
            StartDateTime = DateTime.Today.AddHours(10),
            EndDateTime = DateTime.Today.AddHours(12)
        };

        var buildingResponse = new BuildingResponse.Get
        {
            Building = new BuildingDto.Index
            {
                Id = Guid.NewGuid(),
                BuildingCode = "GSCHB",
                Name = "Gebouw C",
                CampusId = Guid.NewGuid(),
                Address = "Test Address",
                Type = "Educational"
            }
        };

        campusService.GetBuildingByBuildingCodeAsync("GSCHB")
            .Returns(Task.FromResult(Result.Success(buildingResponse)));

        var cut = RenderComponent<ScheduleItem>(parameters => parameters
            .Add(p => p.Schedule, schedule)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { })));

        var roomLink = cut.Find("a");
        roomLink.Click();

        await campusService.Received(1).GetBuildingByBuildingCodeAsync("GSCHB");

        var navMan = Services.GetRequiredService<NavigationManager>();
        var campusId = buildingResponse.Building.CampusId;
        Assert.Contains($"/campus-plan/{campusId}#building-GSCHB", navMan.Uri);
    }

    [Fact]
    public void TruncateTitle_WhenTooLong()
    {
        var longCourse = "This is a very long course title that should definitely be truncated";
        var schedule = new ScheduleDto.Schedule
        {
            Id = "test1",
            Course = longCourse,
            StartDateTime = DateTime.Today.AddHours(10),
            EndDateTime = DateTime.Today.AddHours(12)
        };

        var cut = RenderComponent<ScheduleItem>(parameters => parameters
            .Add(p => p.Schedule, schedule)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { })));
        Assert.Contains("This is a very long course title that", cut.Markup);
        Assert.Contains("...", cut.Markup);
    }

    [Fact]
    public void NotTruncateTitle_WhenShortEnough()
    {
        var shortCourse = "Short Course";
        var schedule = new ScheduleDto.Schedule
        {
            Id = "test1",
            Course = shortCourse,
            StartDateTime = DateTime.Today.AddHours(10),
            EndDateTime = DateTime.Today.AddHours(12)
        };

        var cut = RenderComponent<ScheduleItem>(parameters => parameters
            .Add(p => p.Schedule, schedule)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { })));

        Assert.Contains(shortCourse, cut.Markup);
        Assert.DoesNotContain("...", cut.Markup);
    }

    [Fact]
    public void NotRender_WhenScheduleIsNull()
    {
        var cut = RenderComponent<ScheduleItem>(parameters => parameters
            .Add(p => p.Schedule, (ScheduleDto.Schedule?)null)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { })));

        Assert.Empty(cut.Markup.Trim());
    }
}
