using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.StudentCards;

namespace Rise.Services.StudentCards;

/// <summary>
/// Service for managing student cards.
/// </summary>
public class StudentCardService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider) : IStudentCardService
{
    /// <summary>
    /// Retrieves the current user's ID from the session context.
    /// Throws UnauthorizedAccessException if the user is not authenticated.
    /// </summary>
    private string GetCurrentUserId()
    {
        var userId = sessionContextProvider.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        return userId;
    }

    /// <summary>
    /// Retrieves the student card for the current user.
    /// Determines if the card is still valid based on the expiration date.
    /// </summary>
    /// <param name="ct">CancellationToken to cancel the operation</param>
    /// <returns>Result with StudentCardDto containing card details, or NotFound if no card found</returns>
    public async Task<Result<StudentCardDto>> GetByUserIdAsync(CancellationToken ct = default)
    {
        var currentUserId = GetCurrentUserId();

        var user = await dbContext.StudentCards
            .AsNoTracking()
            .Where(u => u.UserId == currentUserId)
            .Select(u => new
            {
                u.Id,
                u.PersonalNumber,
                u.FirstName,
                u.LastName,
                u.BirthDate,
                u.ExpirationDate,
                u.ProfilePicture
            })
            .FirstOrDefaultAsync(ct);

        if (user == null || string.IsNullOrEmpty(user.PersonalNumber))
        {
            return Result.NotFound();
        }

        return Result.Success(new StudentCardDto
        {
            PersonalNumber = user.PersonalNumber,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            ExpirationDate = user.ExpirationDate,
            ProfilePicture = user.ProfilePicture,
            IsValid = DateTime.Now <= user.ExpirationDate,
        });
    }

}
