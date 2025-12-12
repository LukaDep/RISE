namespace Rise.Client.Contact;

using Rise.Shared.Contact;
using Rise.Client.Tests.Contact;
using Microsoft.Extensions.Localization;

/// <summary>
/// Unit tests for the Contact search functionality.
/// </summary>
public class IndexSearchShould : TestContext
{
    public IndexSearchShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IContactService, FakeContactService>();
    }

    [Fact]
    public void RenderSearchInput()
    {
        var cut = RenderComponent<ContactOverview>();

        // SearchBar always renders an input with the search placeholder
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var input = cut.Find("input[name='search']");
        Assert.NotNull(input);
        Assert.Equal(localizer["Common.SearchPlaceholder"], input.GetAttribute("placeholder"));
    }

    [Fact]
    public void Search_FiltersResults()
    {
        var cut = RenderComponent<ContactOverview>();

        var input = cut.Find("input[name='search']");
        input.Input("John");

        Assert.Contains("John Doe", cut.Markup);
        Assert.DoesNotContain("Jane Smith", cut.Markup);
    }
}
