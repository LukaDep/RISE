using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Rise.Client.Campus.Components;
using Rise.Shared.Campus;
using Xunit;

namespace Rise.Client.Campus.Components;

/// <summary>
/// Unit tests for the <see cref="CampusCard"/> component.
/// Verifies rendering of campus information and user interactions.
/// </summary>
public class CampusCardShould : TestContext
{
    /// <summary>
    /// Initializes a new instance of the test context with localization services.
    /// </summary>
    public CampusCardShould()
    {
        Services.AddLocalization();
    }

    private static CampusDto.Index CreateCampus(
        string name = "Test Campus",
        string street = "Test Street",
        string houseNumber = "1",
        string postalCode = "9000",
        string city = "Test City",
        string contactPhone = "",
        string description = "",
        IEnumerable<string>? facilities = null,
        Guid? id = null)
    {
        return new CampusDto.Index
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Street = street,
            HouseNumber = houseNumber,
            PostalCode = postalCode,
            City = city,
            ContactPhone = contactPhone,
            Description = description,
            Facilities = facilities ?? new List<string>()
        };
    }

    [Fact]
    public void RenderCampusName()
    {
        var campus = CreateCampus(name: "Test Campus");
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        Assert.Contains("Test Campus", cut.Markup);
    }

    [Fact]
    public void RenderLocation()
    {
        var campus = CreateCampus(street: "Test Street", houseNumber: "1", postalCode: "9000", city: "Test City");
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        Assert.Contains("Test Street 1", cut.Markup);
    }

    [Fact]
    public void RenderCityBadge()
    {
        var campus = CreateCampus(city: "Test City");
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        // City should appear in the badge at the top
        Assert.Contains("Test City", cut.Markup);
        Assert.Contains("fa-location-dot", cut.Markup);
    }

    [Fact]
    public void RenderMapIcon()
    {
        var campus = CreateCampus();
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        // The card should show the map location icon
        Assert.Contains("fa-map-location-dot", cut.Markup);
    }

    [Fact]
    public void RenderMapButton()
    {
        var campus = CreateCampus();
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        // Map button with icon should be visible
        Assert.Contains("fa-map", cut.Markup);
    }

    [Fact]
    public void RenderContactPhoneWhenProvided()
    {
        var campus = CreateCampus(contactPhone: "+32 9 123 45 67");
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        Assert.Contains("+32 9 123 45 67", cut.Markup);
        Assert.Contains("fa-phone", cut.Markup);
    }

    [Fact]
    public void NotRenderContactPhoneWhenEmpty()
    {
        var campus = CreateCampus(contactPhone: "");
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        // Phone icon should not appear when no phone is provided
        Assert.DoesNotContain("fa-phone", cut.Markup);
    }

    [Fact]
    public void RenderDescriptionWhenProvided()
    {
        var campus = CreateCampus(description: "This is a test description");
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        Assert.Contains("This is a test description", cut.Markup);
    }

    [Fact]
    public void RenderFacilitiesWhenProvided()
    {
        var facilities = new List<string> { "Library", "Cafeteria", "Gym" };
        var campus = CreateCampus(facilities: facilities);
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        Assert.Contains("Library", cut.Markup);
        Assert.Contains("Cafeteria", cut.Markup);
        Assert.Contains("Gym", cut.Markup);
        Assert.Contains("fa-check-circle", cut.Markup);
    }

    [Fact]
    public void NotRenderFacilitiesSectionWhenEmpty()
    {
        var campus = CreateCampus(facilities: new List<string>());
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        // Facilities label/icon should not appear
        Assert.DoesNotContain("fa-building", cut.Markup);
    }

    [Fact]
    public void ShowCardWhenRendered()
    {
        var campus = CreateCampus(name: "Test Campus");
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        var markup = cut.Markup;
        Assert.Contains("Test Campus", markup);
        // Card should have proper styling
        Assert.Contains("rounded-2xl", markup);
        Assert.Contains("shadow-md", markup);
    }

    [Fact]
    public void NavigateToCampusPlanWhenMapButtonClicked()
    {
        var campusId = Guid.NewGuid();
        var campus = CreateCampus(id: campusId);
        var navManager = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();

        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        // Find the map button and click it
        var mapButton = cut.Find("button.w-full");
        mapButton.Click();

        Assert.Contains($"/campus-plan/{campusId}", navManager.Uri);
    }

    [Fact]
    public void RenderCardSuccessfully()
    {
        var navManager = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        var campus = CreateCampus(name: "Test Campus");
        
        var cut = RenderComponent<CampusCard>(parameters => parameters
            .Add(p => p.Campus, campus));

        Assert.Contains("Test Campus", cut.Markup);

        navManager.NavigateTo("/campus");

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(cut.Markup);
        });
    }
}
