namespace Rise.Client.Contact;

using Rise.Shared.Contact;
using Rise.Client.Tests.Contact;
using Microsoft.Extensions.Localization;

public class OverviewShould : TestContext
{
    public OverviewShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IContactService, FakeContactService>();
    }

    [Fact]
    public void GroupsByType_And_ShowsCounts()
    {
        var cut = RenderComponent<ContactOverview>();

        // There should be group headers for Academic and Administrative
        Assert.Contains("Academic", cut.Markup);
        Assert.Contains("Administrative", cut.Markup);

        // The counts are rendered in a small rounded span; ensure numeric counts present
        Assert.Matches("\\d+", cut.FindAll("span").FirstOrDefault(s => s.ClassName.Contains("rounded-full"))?.TextContent ?? "");
    }

    [Fact]
    public void ClickingFilterChip_FiltersResults()
    {
        var cut = RenderComponent<ContactOverview>();

        // find the chip button with Administrative text
        var adminBtn = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Trim() == "Administrative");
        Assert.NotNull(adminBtn);

        adminBtn.Click();

        // after clicking filter, only admin contacts remain
        Assert.Contains("Jane Smith", cut.Markup);
        Assert.DoesNotContain("John Doe", cut.Markup);
    }

    [Fact]
    public void ClickingListItem_ExpandsDetails()
    {
        var cut = RenderComponent<ContactOverview>();

        // find the first ContactListItem container (it has class group and is clickable)
        var item = cut.FindAll("div.group").FirstOrDefault();
        Assert.NotNull(item);

        // click it to expand
        item.Click();

        // expanded content should contain either phone or email or contact person
        Assert.True(cut.Markup.Contains("tel:") || cut.Markup.Contains("mailto:") || cut.Markup.Contains("Coordinator"));
    }
}
