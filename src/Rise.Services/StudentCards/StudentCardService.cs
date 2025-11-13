using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.StudentCards;

namespace Rise.Services.StudentCards;

/// <summary>
/// Service for managing Student Cards stored in the User table.
/// </summary>
public class StudentCardService : IStudentCardService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ISessionContextProvider _sessionContextProvider;

    public StudentCardService(
        ApplicationDbContext dbContext,
        ISessionContextProvider sessionContextProvider)
    {
        _dbContext = dbContext;
        _sessionContextProvider = sessionContextProvider;
    }

    private string GetCurrentUserId()
    {
        var userId = _sessionContextProvider.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        return userId;
    }

    public async Task<Result<StudentCardDto>> GetStudentCardByIdAsync(Guid id, CancellationToken ct = default)
    {
        var currentUserId = GetCurrentUserId();

        // Verify that the requested ID matches the current user's ID
        if (id.ToString() != currentUserId)
        {
            return Result.Forbidden("You can only access your own student card");
        }

        var user = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == currentUserId)
            .Select(u => new
            {
                u.Id,
                PersonalNumber = EF.Property<string>(u, "PersonalNumber"),
                FirstName = EF.Property<string>(u, "FirstName"),
                LastName = EF.Property<string>(u, "LastName"),
                BirthDate = EF.Property<DateTime?>(u, "BirthDate"),
                ExpirationDate = EF.Property<DateTime?>(u, "ExpirationDate"),
                ProfilePicture = EF.Property<string>(u, "ProfilePicture"),
                BarcodeData = EF.Property<string>(u, "BarcodeData")
            })
            .FirstOrDefaultAsync();

        if (user == null || string.IsNullOrEmpty(user.PersonalNumber))
        {
            return Result.NotFound();
        }

        return Result.Success(new StudentCardDto
        {
            PersonalNumber = user.PersonalNumber,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            ExpirationDate = user.ExpirationDate ?? default,
            ProfilePicture = user.ProfilePicture,
            IsValid = user.ExpirationDate.HasValue && DateTime.UtcNow <= user.ExpirationDate.Value,
        });
    }

}
