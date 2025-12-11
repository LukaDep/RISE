namespace Rise.Client.Contact;

using Rise.Shared.Contact;
using Rise.Client.Tests.Contact;
using Microsoft.AspNetCore.Components;

public class ComponentsShould : TestContext
{
    public ComponentsShould()
    {
        Services.AddLocalization();
    }
    [Fact]
    public void ContactCardDetails_Renders_Info_And_Close()
    {
        var closed = false;
        var cut = RenderComponent<Rise.Client.Contact.Components.ContactCardDetails>(parameters =>
            parameters.Add(p => p.Name, "Detail Name")
                      .Add(p => p.Type, "Campus")
                      .Add(p => p.PhoneNumber, "+100")
                      .Add(p => p.Email, "a@b.com")
                      .Add(p => p.ContactPerson, "Bob")
                      .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => closed = true))
        );

        Assert.Contains("Detail Name", cut.Markup);
        Assert.Contains("Bob", cut.Markup);
        Assert.Contains("tel:+100", cut.Markup);
        Assert.Contains("mailto:a@b.com", cut.Markup);

        // close button should trigger callback
        var btn = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Trim() == "X");
        Assert.NotNull(btn);
        btn.Click();
        Assert.True(closed);
    }

    [Fact]
    public void ContactCard_Renders_Name_Type_And_Action_Links()
    {
        var cut = RenderComponent<Rise.Client.Contact.Components.ContactCard>(parameters =>
            parameters.Add(p => p.Name, "Card Name")
                      .Add(p => p.Type, "Departement")
                      .Add(p => p.PhoneNumber, "+200")
                      .Add(p => p.Email, "c@d.com")
        );

        Assert.Contains("Card Name", cut.Markup);
        Assert.Contains("Departement", cut.Markup);
        Assert.Contains("tel:+200", cut.Markup);
        Assert.Contains("mailto:c@d.com", cut.Markup);
    }

    [Fact]
    public void ContactTypeIcon_Maps_To_Correct_Icon()
    {
        var cut = RenderComponent<Rise.Client.Contact.Components.ContactTypeIcon>(parameters => parameters.Add(p => p.Type, "Organisatie"));
        Assert.Contains("fa-building", cut.Markup);

        var def = RenderComponent<Rise.Client.Contact.Components.ContactTypeIcon>(parameters => parameters.Add(p => p.Type, "Unknown"));
        Assert.Contains("fa-address-book", def.Markup);
    }

    [Fact]
    public void ContactListItem_QuickAction_Links_Exist()
    {
        var contact = new ContactDto.Index { Id = Guid.NewGuid(), Name = "QA", Type = "Academic", PhoneNumber = "+300", Email = "qa@example.org" };
        var cut = RenderComponent<Rise.Client.Contact.Components.ContactListItem>(parameters => parameters.Add(p => p.Contact, contact));

        // when collapsed, quick action links present
        Assert.Contains("tel:+300", cut.Markup);
        Assert.Contains("mailto:qa@example.org", cut.Markup);
    }
}
