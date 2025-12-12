using System.Security.Claims;
using Rise.Domain.StudentCards;
using Rise.Persistence;
using Rise.Services.StudentCards;
using Rise.Services.Tests.Fakers;
using Rise.Services.Tests.TestInfrastructure;
using Microsoft.AspNetCore.Identity;

namespace Rise.Services.Tests.StudentCards;

public class StudentCardServiceShould
{
    private static ClaimsPrincipal CreateUserPrincipal(string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    private static async Task CreateIdentityUserAsync(ApplicationDbContext dbContext, string userId)
    {
        var user = new IdentityUser
        {
            Id = userId,
            UserName = $"testuser_{userId}@example.com",
            NormalizedUserName = $"TESTUSER_{userId}@EXAMPLE.COM",
            Email = $"testuser_{userId}@example.com",
            NormalizedEmail = $"TESTUSER_{userId}@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetByUserIdAsync_WithValidStudentCard_ShouldReturnSuccess()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        await CreateIdentityUserAsync(dbContext, userId);

        var studentCard = new StudentCard(
            userId: userId,
            personalNumber: "123456789",
            firstName: "John",
            lastName: "Doe",
            birthDate: new DateTime(2000, 1, 15),
            expirationDate: DateTime.Now.AddYears(1),
            profilePicture: "https://example.com/profile.jpg"
        );
        dbContext.StudentCards.Add(studentCard);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("123456789", result.Value.PersonalNumber);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.True(result.Value.IsValid);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithExpiredCard_ShouldReturnIsValidFalse()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        await CreateIdentityUserAsync(dbContext, userId);

        var studentCard = new StudentCard(
            userId: userId,
            personalNumber: "987654321",
            firstName: "Jane",
            lastName: "Smith",
            birthDate: new DateTime(1999, 5, 20),
            expirationDate: DateTime.Now.AddDays(-30), // Expired
            profilePicture: null
        );
        dbContext.StudentCards.Add(studentCard);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(result.Value.IsValid);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNoStudentCard_ShouldReturnNotFound()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        await CreateIdentityUserAsync(dbContext, userId);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithUnauthenticatedUser_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(new ClaimsPrincipal()); // No claims
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetByUserIdAsync());
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNullUser_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(null!);
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetByUserIdAsync());
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnCorrectProfilePicture()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        await CreateIdentityUserAsync(dbContext, userId);

        var profilePicUrl = "https://storage.example.com/student-photos/12345.png";
        var studentCard = new StudentCard(
            userId: userId,
            personalNumber: "111222333",
            firstName: "Alice",
            lastName: "Johnson",
            birthDate: new DateTime(2001, 8, 10),
            expirationDate: DateTime.Now.AddMonths(6),
            profilePicture: profilePicUrl
        );
        dbContext.StudentCards.Add(studentCard);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(profilePicUrl, result.Value.ProfilePicture);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNullProfilePicture_ShouldReturnNull()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        await CreateIdentityUserAsync(dbContext, userId);

        var studentCard = new StudentCard(
            userId: userId,
            personalNumber: "444555666",
            firstName: "Bob",
            lastName: "Brown",
            birthDate: new DateTime(2002, 3, 25),
            expirationDate: DateTime.Now.AddYears(2),
            profilePicture: null
        );
        dbContext.StudentCards.Add(studentCard);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.ProfilePicture);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnCorrectExpirationDate()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        await CreateIdentityUserAsync(dbContext, userId);

        var expirationDate = new DateTime(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc);
        var studentCard = new StudentCard(
            userId: userId,
            personalNumber: "777888999",
            firstName: "Charlie",
            lastName: "Davis",
            birthDate: new DateTime(2003, 12, 5),
            expirationDate: expirationDate,
            profilePicture: null
        );
        dbContext.StudentCards.Add(studentCard);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expirationDate, result.Value.ExpirationDate);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithDifferentUserId_ShouldNotReturnOtherUsersCard()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        await CreateIdentityUserAsync(dbContext, userId1);
        await CreateIdentityUserAsync(dbContext, userId2);

        // Create card for user 1
        var studentCard = new StudentCard(
            userId: userId1,
            personalNumber: "111111111",
            firstName: "User",
            lastName: "One",
            birthDate: new DateTime(2000, 1, 1),
            expirationDate: DateTime.Now.AddYears(1),
            profilePicture: null
        );
        dbContext.StudentCards.Add(studentCard);
        await dbContext.SaveChangesAsync();

        // Try to access with user 2
        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId2));
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert - user 2 should not get user 1's card
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithCardExpiringToday_ShouldBeValid()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        await CreateIdentityUserAsync(dbContext, userId);

        var studentCard = new StudentCard(
            userId: userId,
            personalNumber: "999000111",
            firstName: "Today",
            lastName: "Expiry",
            birthDate: new DateTime(2000, 6, 15),
            expirationDate: DateTime.Now.Date.AddHours(23).AddMinutes(59), // Today
            profilePicture: null
        );
        dbContext.StudentCards.Add(studentCard);
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new StudentCardService(dbContext, sessionProvider);

        // Act
        var result = await service.GetByUserIdAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.IsValid);
    }
}
