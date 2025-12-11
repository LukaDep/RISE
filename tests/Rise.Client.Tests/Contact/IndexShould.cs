namespace Rise.Client.Contact;

using Rise.Shared.Contact;
using Microsoft.Extensions.Localization;
using Rise.Client.Tests.Contact;
using Rise.Client.Components;

public class IndexShould : TestContext
{
    public IndexShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IContactService, FakeContactService>();
    }

    [Fact]
    public void RendersHeaderAndSearchElements()
    {
        var cut = RenderComponent<ContactOverview>();

        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        // Page title header comes from BasePage Title -> ensure localized title present
        Assert.Contains(localizer["Contact.Title"], cut.Markup);

        // Search bar toggle/input behavior: input may be hidden initially
        var buttons = cut.FindAll("button");
        Assert.NotEmpty(buttons);
    }

    [Fact]
    public void ShowsNotFoundWhenEmpty()
    {
        Services.AddScoped<IContactService, NullContactService>();
        var cut = RenderComponent<ContactOverview>();

        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Contact.NotFound"], cut.Markup);
    }
}
