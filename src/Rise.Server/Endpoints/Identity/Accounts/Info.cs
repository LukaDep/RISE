using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Rise.Shared.Identity.Accounts;
using Rise.Shared.StudentCards;

namespace Rise.Server.Endpoints.Identity.Accounts;

/// <summary>
/// Get the logged in user info, Roles, Claims, etc.
/// </summary>
/// <param name="userManager"></param>
public class Info(UserManager<IdentityUser> userManager, IStudentCardService studentCardService) : EndpointWithoutRequest<Result<AccountResponse.Info>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/identity/accounts/info");
    }

    /// <summary>
    /// Retrieves the current user's account information including roles, claims, and student card.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the user's account information.</returns>
    public override async Task<Result<AccountResponse.Info>> ExecuteAsync(CancellationToken ct)
    {
        if (await userManager.GetUserAsync(HttpContext.User) is not { } user)
        {
            return Result.NotFound();
        }

        return Result.Success(await CreateInfoResponseAsync(user, HttpContext.User));
    }

    /// <summary>
    /// Creates the account info response DTO from the user and claims principal.
    /// </summary>
    /// <param name="user">The identity user.</param>
    /// <param name="claimsPrincipal">The claims principal containing user claims.</param>
    /// <returns>The account info response.</returns>
    private async Task<AccountResponse.Info> CreateInfoResponseAsync(IdentityUser user, ClaimsPrincipal claimsPrincipal)
    {
        var studentCard = await studentCardService.GetByUserIdAsync(CancellationToken.None);
        return new()
        {
            Email = user.Email!,
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
            Claims = claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value),
            Roles = claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
            StudentCard = studentCard.IsSuccess ? studentCard.Value : null
        };
    }
}