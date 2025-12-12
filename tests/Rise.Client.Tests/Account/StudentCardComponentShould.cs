using Ardalis.Result;
using Rise.Client.Account.Components;
using Rise.Shared.StudentCards;

namespace Rise.Client.Account;

public class StudentCardComponentShould : TestContext
{
    public StudentCardComponentShould()
    {
        Services.AddLocalization();
        JSInterop.SetupVoid("scaleStudentCard", _ => true);
    }

    [Fact]
    public void RenderStudentCardWithValidData()
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
        var cut = RenderComponent<StudentCard>();

        // Assert
        Assert.Contains("123456789", cut.Markup);
        Assert.Contains("John", cut.Markup);
        Assert.Contains("Doe", cut.Markup);
    }

    [Fact]
    public void RenderProfilePicture()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "123456789",
            FirstName = "John",
            LastName = "Doe",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = "https://example.com/my-photo.jpg",
            IsValid = true
        };
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(studentCard));

        // Act
        var cut = RenderComponent<StudentCard>();

        // Assert
        var img = cut.Find("img");
        Assert.Equal("https://example.com/my-photo.jpg", img.GetAttribute("src"));
    }

    [Fact]
    public void RenderQRCode()
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
        var cut = RenderComponent<StudentCard>();

        // Assert - QR code should be rendered as base64 image
        var images = cut.FindAll("img");
        var qrImage = images.FirstOrDefault(img => img.GetAttribute("alt") == "Student QR Code");
        Assert.NotNull(qrImage);
        Assert.Contains("data:image/png;base64,", qrImage.GetAttribute("src"));
    }

    [Fact]
    public void RenderEuropeanStudentCardText()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "123456789",
            FirstName = "Test",
            LastName = "User",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = "https://example.com/profile.jpg",
            IsValid = true
        };
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(studentCard));

        // Act
        var cut = RenderComponent<StudentCard>();

        // Assert
        Assert.Contains("European Student Card", cut.Markup);
    }

    [Fact]
    public void RenderStudentIdentityCardLabel()
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
        var cut = RenderComponent<StudentCard>();

        // Assert
        Assert.Contains("identity card", cut.Markup);
    }

    [Fact]
    public void RenderGraduationCapIcon()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "111222333",
            FirstName = "Alice",
            LastName = "Johnson",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = null,
            IsValid = true
        };
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(studentCard));

        // Act
        var cut = RenderComponent<StudentCard>();

        // Assert
        Assert.Contains("fa-graduation-cap", cut.Markup);
    }

    [Fact]
    public void RenderEuropeanFlag()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "444555666",
            FirstName = "Bob",
            LastName = "Brown",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = null,
            IsValid = true
        };
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(studentCard));

        // Act
        var cut = RenderComponent<StudentCard>();

        // Assert
        Assert.Contains("Flag_of_Europe.png", cut.Markup);
    }

    [Fact]
    public void CallScaleStudentCardJsOnRender()
    {
        // Arrange
        var studentCard = new StudentCardDto
        {
            PersonalNumber = "777888999",
            FirstName = "Charlie",
            LastName = "Davis",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ProfilePicture = null,
            IsValid = true
        };
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(studentCard));

        // Act
        var cut = RenderComponent<StudentCard>();

        // Assert - JS interop should be invoked
        var invocations = JSInterop.Invocations;
        Assert.Contains(invocations, inv => inv.Identifier == "scaleStudentCard");
    }
}

public class StudentCardComponentWithNullDataShould : TestContext
{
    public StudentCardComponentWithNullDataShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IStudentCardService>(_ => new FakeStudentCardService(null));
        JSInterop.SetupVoid("scaleStudentCard", _ => true);
    }

    [Fact]
    public void ShowLoadingWhenNoStudentCard()
    {
        // Act
        var cut = RenderComponent<StudentCard>();

        // Assert - should show loading component
        Assert.Contains("animate-spin", cut.Markup);
    }
}

public class FakeStudentCardService : IStudentCardService
{
    private readonly StudentCardDto? _studentCard;

    public FakeStudentCardService(StudentCardDto? studentCard)
    {
        _studentCard = studentCard;
    }

    public Task<Result<StudentCardDto>> GetByUserIdAsync(CancellationToken ct = default)
    {
        if (_studentCard == null)
            return Task.FromResult(Result<StudentCardDto>.NotFound());
        return Task.FromResult(Result.Success(_studentCard));
    }
}
