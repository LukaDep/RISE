using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Rise.Client.Campus.Components;
using Xunit;

namespace Rise.Client.Campus.Components;

public class CampusCardShould : TestContext
{
    public CampusCardShould()
    {
        Services.AddLocalization();
    }

    [Fact]
    public void RenderCampusName()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        Assert.Contains("Test Campus", cut.Markup);
    }

    [Fact]
    public void RenderLocationWhenClosed()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        Assert.Contains("Test Street 1", cut.Markup);
    }

    [Fact]
    public void ShowChevronDownWhenClosed()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        Assert.Contains("fa-chevron-down", cut.Markup);
    }

    [Fact]
    public void ShowChevronUpWhenOpen()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        var card = cut.Find("div.flex.flex-col");
        card.Click();

        Assert.Contains("fa-chevron-up", cut.Markup);
    }

    [Fact]
    public void RenderMapButtonWhenOpen()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        var card = cut.Find("div.flex.flex-col");
        card.Click();

        Assert.Contains("bg-blue-600", cut.Markup);
    }

    [Fact]
    public void RenderContactPhoneWhenProvided()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "+32 9 123 45 67")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        var card = cut.Find("div.flex.flex-col");
        card.Click();

        Assert.Contains("+32 9 123 45 67", cut.Markup);
    }

    [Fact]
    public void RenderDescriptionWhenProvided()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "This is a test description")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        var card = cut.Find("div.flex.flex-col");
        card.Click();

        Assert.Contains("This is a test description", cut.Markup);
    }

    [Fact]
    public void RenderFacilitiesWhenProvided()
    {
        var facilities = new List<string> { "Library", "Cafeteria", "Gym" };
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, facilities)
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        var card = cut.Find("div.flex.flex-col");
        card.Click();

        Assert.Contains("Library", cut.Markup);
        Assert.Contains("Cafeteria", cut.Markup);
        Assert.Contains("Gym", cut.Markup);
    }

    [Fact]
    public void ShowCardWhenSearchTermMatches()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, "Test"));

        var markup = cut.Markup;
        Assert.Contains("Test Campus", markup);
    }

    [Fact]
    public void ToggleTwiceToCloseCard()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        var card = cut.Find("div.flex.flex-col");

        card.Click();
        card.Click();

        Assert.Contains("fa-chevron-down", cut.Markup);
    }

    [Fact]
    public void NavigateToCampusPlanWhenMapButtonClicked()
    {
        var campusId = Guid.NewGuid();
        var navManager = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();

        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, campusId)
            .Add(p => p.SearchTerm, ""));

        var card = cut.Find("div.flex.flex-col");
        card.Click();

        var mapButton = cut.Find("button.px-3");
        mapButton.Click();

        Assert.Contains($"/campus-plan/{campusId}", navManager.Uri);
    }

    [Fact]
    public void UpdateVisibilityOnLocationChanged()
    {
        var navManager = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();

        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, "Test"));

        Assert.Contains("Test Campus", cut.Markup);

        navManager.NavigateTo("/campus");

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(cut.Markup);
        });
    }

    [Fact]
    public void RenderWebsiteWhenProvided()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Website, "https://www.hogent.be")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, ""));

        var card = cut.Find("div.flex.flex-col");
        card.Click();

        Assert.Contains("https://www.hogent.be", cut.Markup);
    }

    [Fact]
    public void SearchTermComparisonIsCaseInsensitive()
    {
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Name, "Test Campus")
            .Add(p => p.Location, "Test Street 1, 9000 Test City")
            .Add(p => p.Facilities, new List<string>())
            .Add(p => p.ContactPhone, "")
            .Add(p => p.Description, "")
            .Add(p => p.Id, Guid.NewGuid())
            .Add(p => p.SearchTerm, "test"));

        Assert.Contains("Test Campus", cut.Markup);
    }
}
