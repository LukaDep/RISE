using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.StudentCards;

namespace Rise.Services.StudentCards;

/// <summary>
/// Service for managing Student Cards stored in the StudentCards table.
/// </summary>
public class StudentCardService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider) : IStudentCardService
{

    private string GetCurrentUserId()
    {
        var userId = sessionContextProvider.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        return userId;
    }

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
