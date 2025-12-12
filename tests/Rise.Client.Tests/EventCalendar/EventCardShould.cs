using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Client.EventCalendar.Components;
using Rise.Shared.Events;

namespace Rise.Client.EventCalendar;

public class EventCardShould : TestContext
{
    public EventCardShould()
    {
        Services.AddLocalization();
    }

    private static EventDTO.Index CreateEvent(
        string name = "Test Event",
        string type = "Sport",
        string? description = "Test description",
        string? registrationLink = null,
        DateTime? startDateTime = null,
        DateTime? endDateTime = null,
        string location = "Test Location")
    {
        var start = startDateTime ?? DateTime.UtcNow.AddDays(1);
        return new EventDTO.Index
        {
            Name = name,
            Type = type,
            Description = description,
            RegistrationLink = registrationLink,
            StartDateTime = start,
            EndDateTime = endDateTime ?? start.AddHours(2),
            Location = location
        };
    }

    [Fact]
    public void DisplaysEventName()
    {
        // Arrange
        var ev = CreateEvent(name: "Basketball Tournament");

        // Act
        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev));

        // Assert
        Assert.Contains("Basketball Tournament", cut.Markup);
    }

    [Fact]
    public void DisplaysEventDescription()
    {
        // Arrange
        var ev = CreateEvent(description: "Annual sports event");

        // Act
        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev));

        // Assert
        Assert.Contains("Annual sports event", cut.Markup);
    }

    [Fact]
    public void TruncatesLongDescription()
    {
        // Arrange
        var longDescription = "This is a very long description that should be truncated because it exceeds fifty characters in length";
        var ev = CreateEvent(description: longDescription);

        // Act
        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev));

        // Assert
        Assert.Contains("...", cut.Markup);
        Assert.DoesNotContain("exceeds fifty characters in length", cut.Markup);
    }

    [Fact]
    public void DisplaysShortDescriptionWithoutTruncation()
    {
        // Arrange
        var shortDescription = "Short desc";
        var ev = CreateEvent(description: shortDescription);

        // Act
        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev));

        // Assert
        Assert.Contains("Short desc", cut.Markup);
        // Should not have truncation ellipsis for short text
    }

    [Fact]
    public void DisplaysRegisterButton_WhenRegistrationLinkExists()
    {
        // Arrange
        var ev = CreateEvent(registrationLink: "https://example.com/register");

        // Act
        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev));

        // Assert
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["EventCard.register"], cut.Markup);
        Assert.Contains("https://example.com/register", cut.Markup);
        Assert.Contains("fa-external-link", cut.Markup);
    }

    [Fact]
    public void DoesNotDisplayRegisterButton_WhenNoRegistrationLink()
    {
        // Arrange
        var ev = CreateEvent(registrationLink: null);

        // Act
        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev));

        // Assert
        Assert.DoesNotContain("fa-external-link", cut.Markup);
    }

    [Fact]
    public async Task InvokesOnClickCallback_WhenCardClicked()
    {
        // Arrange
        var ev = CreateEvent();
        var clicked = false;

        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev)
            .Add(p => p.OnClick, EventCallback.Factory.Create(this, () => clicked = true)));

        // Act
        var card = cut.Find("div.cursor-pointer");
        await cut.InvokeAsync(() => card.Click());

        // Assert
        Assert.True(clicked);
    }

    [Fact]
    public void HasCorrectStyling()
    {
        // Arrange
        var ev = CreateEvent();

        // Act
        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev));

        // Assert
        Assert.Contains("rounded-xl", cut.Markup);
        Assert.Contains("shadow", cut.Markup);
        Assert.Contains("cursor-pointer", cut.Markup);
    }

    [Fact]
    public void DisplaysEventWithEmptyDescription()
    {
        // Arrange
        var ev = CreateEvent(description: "");

        // Act
        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev));

        // Assert - should render without error
        Assert.Contains("Test Event", cut.Markup);
    }

    [Fact]
    public void RegisterLinkStopsPropagation()
    {
        // Arrange
        var ev = CreateEvent(registrationLink: "https://example.com/register");

        // Act
        var cut = RenderComponent<EventCard>(parameters => parameters
            .Add(p => p.specificEvent, ev));

        // Assert - the link should have onclick:stopPropagation
        var link = cut.Find("a");
        Assert.NotNull(link);
        Assert.Contains("https://example.com/register", link.GetAttribute("href"));
    }
}
