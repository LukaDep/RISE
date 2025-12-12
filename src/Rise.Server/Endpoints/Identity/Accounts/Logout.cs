using Microsoft.AspNetCore.Identity;

namespace Rise.Server.Endpoints.Identity.Accounts;

/// <summary>
/// Logout Endpoint.
/// </summary>
/// <param name="signInManager"></param>
public class Logout(SignInManager<IdentityUser> signInManager) : EndpointWithoutRequest
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Post("/api/identity/accounts/logout");
    }

    /// <summary>
    /// Signs out the current user and invalidates their session.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the logout was successful.</returns>
    public override async Task<Result> HandleAsync(CancellationToken ct)
    {
        await signInManager.SignOutAsync();
        return Result.NoContent();
    }
}