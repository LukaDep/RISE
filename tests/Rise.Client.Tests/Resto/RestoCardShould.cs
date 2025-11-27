using Rise.Client.Resto.Components;
using Rise.Shared.Resto;

namespace Rise.Client.Resto;

public class RestoCardShould : TestContext
{
    public RestoCardShould()
    {
        Services.AddLocalization();
    }

    [Fact]
    public void RenderBasicRestoInformation()
    {
        var resto = new RestoDto.Index
        {
            Id = Guid.NewGuid(),
            Name = "Campus Cafe",
            BuildingId = Guid.NewGuid(),
            Description = "A cozy cafe",
            PhoneNumber = "09 123 45 67",
            Email = "cafe@hogent.be"
        };
        var cut = RenderComponent<RestoCard>(parameters => parameters
            .Add(p => p.Resto, resto));
        Assert.Contains("Campus Cafe", cut.Markup);
        Assert.Contains("A cozy cafe", cut.Markup);
        Assert.Contains("09 123 45 67", cut.Markup);
        Assert.Contains("cafe@hogent.be", cut.Markup);
    }

    [Fact]
    public void ShowOpenStatusWithGreenBadge()
    {
        var now = DateTime.Now;
        var resto = new RestoDto.Index
        {
            Id = Guid.NewGuid(),
            Name = "Test Resto",
            BuildingId = Guid.NewGuid(),
            OpeningHours = new Dictionary<DayOfWeek, string>
            {
                { now.DayOfWeek, $"{now.AddHours(-1):HH:mm}-{now.AddHours(2):HH:mm}" }
            }
        };
        var cut = RenderComponent<RestoCard>(parameters => parameters
            .Add(p => p.Resto, resto));
        Assert.Contains("bg-green-500", cut.Markup);
        Assert.Contains("animate-pulse", cut.Markup);
    }

    [Fact]
    public void ShowClosedStatusWithRedBadge()
    {
        var now = DateTime.Now;
        var resto = new RestoDto.Index
        {
            Id = Guid.NewGuid(),
            Name = "Test Resto",
            BuildingId = Guid.NewGuid(),
            OpeningHours = new Dictionary<DayOfWeek, string>
            {
                { now.DayOfWeek, $"{now.AddHours(-3):HH:mm}-{now.AddHours(-1):HH:mm}" }
            }
        };
        var cut = RenderComponent<RestoCard>(parameters => parameters
            .Add(p => p.Resto, resto));
        Assert.Contains("bg-red-500", cut.Markup);
    }

    [Fact]
    public void RenderNavigationLinkToMenu()
    {
        var restoId = Guid.NewGuid();
        var resto = new RestoDto.Index
        {
            Id = restoId,
            Name = "Test Resto",
            BuildingId = Guid.NewGuid()
        };
        var cut = RenderComponent<RestoCard>(parameters => parameters
            .Add(p => p.Resto, resto));
        Assert.Contains($"/resto/{restoId}/menu", cut.Markup);
    }

    [Fact]
    public void ToggleOpeningHoursVisibility()
    {
        var resto = new RestoDto.Index
        {
            Id = Guid.NewGuid(),
            Name = "Test Resto",
            BuildingId = Guid.NewGuid(),
            OpeningHours = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "09:00-17:00" }
            }
        };
        var cut = RenderComponent<RestoCard>(parameters => parameters
            .Add(p => p.Resto, resto));
        Assert.DoesNotContain("09:00-17:00", cut.Markup);
        var button = cut.Find("button");
        button.Click();
        Assert.Contains("09:00-17:00", cut.Markup);
    }
}
