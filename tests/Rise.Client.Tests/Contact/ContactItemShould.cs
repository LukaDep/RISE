namespace Rise.Client.Contact;

using Rise.Shared.Contact;
using Rise.Client.Tests.Contact;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
public class ContactItemShould : TestContext
{
    private readonly ContactDto.Index _contact = new()
    {
        Id = Guid.NewGuid(),
        Type = "Academic",
        Name = "Test Person",
        Email = "test.person@example.org",
        PhoneNumber = "+1000",
        ContactPerson = "Coordinator",
        Campusses = new[] { "Main" }
    };

    public ContactItemShould()
    {
        Services.AddLocalization();
    }

    [Fact]
    public void RendersContactSummary()
    {
        var cut = RenderComponent<Rise.Client.Contact.Components.ContactListItem>(parameters => parameters.Add(p => p.Contact, _contact));

        Assert.Contains(_contact.Name, cut.Markup);
        // ensure at least one of the quick action buttons (phone/email) or email text is present
        Assert.True(cut.Markup.Contains("fa-phone") || cut.Markup.Contains("fa-envelope") || cut.Markup.Contains(_contact.Email));
    }

    [Fact]
    public void TogglesExpandedDetails()
    {
        var expanded = false;
        var receiver = new object();
        var toggleCallback = EventCallback.Factory.Create(receiver, () => expanded = !expanded);

        var cut = RenderComponent<Rise.Client.Contact.Components.ContactListItem>(parameters =>
            parameters.Add(p => p.Contact, _contact)
                      .Add(p => p.IsExpanded, false)
                      .Add(p => p.OnToggle, toggleCallback)
        );

        // click to toggle (click header div)
        cut.Find("div").Click();
        Assert.True(expanded);
    }
}
