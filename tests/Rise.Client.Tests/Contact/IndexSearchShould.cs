namespace Rise.Client.Contact;

using Rise.Shared.Contact;
using Rise.Client.Tests.Contact;
using Microsoft.Extensions.Localization;

public class IndexSearchShould : TestContext
{
    public IndexSearchShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IContactService, FakeContactService>();
    }

    [Fact]
    public void ToggleSearch_RendersInput()
    {
        var cut = RenderComponent<ContactOverview>();

        // find the search toggle button (SearchBar renders a button with magnifying icon)
        var searchButton = cut.FindAll("button").FirstOrDefault(b => b.OuterHtml.Contains("fa-magnifying-glass") || b.OuterHtml.Contains("fa-xmark"));
        Assert.NotNull(searchButton);

        // initially no input
        Assert.Empty(cut.FindAll("input"));

        // open search
        searchButton.Click();

        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var input = cut.Find("input");
        Assert.Equal(localizer["Common.SearchPlaceholder"], input.GetAttribute("placeholder"));
    }

    [Fact]
    public void Search_FiltersResults()
    {
        var cut = RenderComponent<ContactOverview>();

        var searchButton = cut.FindAll("button").FirstOrDefault(b => b.OuterHtml.Contains("fa-magnifying-glass") || b.OuterHtml.Contains("fa-xmark"));
        searchButton.Click();

        var input = cut.Find("input");
        input.Input("John");

        Assert.Contains("John Doe", cut.Markup);
        Assert.DoesNotContain("Jane Smith", cut.Markup);
    }
}
