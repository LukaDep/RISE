using Ardalis.Result;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Rise.Shared.StudentCards;
using System.Security.Claims;

namespace Rise.Client.Account;

public class IndexShould : TestContext
{
    public IndexShould()
    {
        Services.AddLocalization();
        JSInterop.SetupVoid("scaleStudentCard", _ => true);

        // Setup authorization
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");
        authContext.SetClaims(new Claim(ClaimTypes.Name, "TestUser"));
    }

    [Fact]
    public void RenderAccountPageWithStudentCard()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "123456789",
            FirstName = "John",
            LastName = "Doe",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = "https://example.com/profile.jpg",
            IsValid = true
        };
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(studentCard));

        // Act
        var cut = RenderComponent<Index>();

        // Assert - Page should contain the student card component
        Assert.Contains("123456789", cut.Markup);
        Assert.Contains("John", cut.Markup);
        Assert.Contains("Doe", cut.Markup);
    }

    [Fact]
    public void RenderWithProperLayout()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "987654321",
            FirstName = "Jane",
            LastName = "Smith",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = null,
            IsValid = true
        };
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(studentCard));

        // Act
        var cut = RenderComponent<Index>();

        // Assert - Check layout classes
        Assert.Contains("flex justify-center", cut.Markup);
        Assert.Contains("bg-white", cut.Markup);
    }

    [Fact]
    public void RenderStudentCardComponent()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "111222333",
            FirstName = "Test",
            LastName = "User",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = null,
            IsValid = true
        };
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(studentCard));

        // Act
        var cut = RenderComponent<Index>();

        // Assert - StudentCard component should be rendered
        var studentCardComponent = cut.FindComponent<Rise.Client.Account.Components.StudentCard>();
        Assert.NotNull(studentCardComponent);
    }

    [Fact]
    public void RenderWithFullWidthContainer()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "444555666",
            FirstName = "Alice",
            LastName = "Johnson",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = null,
            IsValid = true
        };
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(studentCard));

        // Act
        var cut = RenderComponent<Index>();

        // Assert
        Assert.Contains("w-full", cut.Markup);
    }
}
