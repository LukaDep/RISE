using Microsoft.AspNetCore.Identity;
using Rise.Shared.Identity.Accounts;

namespace Rise.Server.Endpoints.Identity.Accounts;

/// <summary>
/// Login Endpoint.
/// </summary>
/// <param name="signInManager"></param>
public class Login(SignInManager<IdentityUser> signInManager) : Endpoint<AccountRequest.Login, Result>
{
    private const bool UseCookies = true;
    private const bool UseSessionCookies = true;

    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Post("/api/identity/accounts/login");
        AllowAnonymous();
    }

    /// <summary>
    /// Authenticates a user with email and password, supporting two-factor authentication.
    /// </summary>
    /// <param name="req">The login request containing credentials.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result indicating success or failure of the login attempt.</returns>
    public override async Task<Result> ExecuteAsync(AccountRequest.Login req, CancellationToken ctx)
    {
        var useCookieScheme = UseCookies || UseSessionCookies;
        var isPersistent = UseCookies && (UseSessionCookies != true);
        signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        var result = await signInManager.PasswordSignInAsync(req.Email!, req.Password!, isPersistent, lockoutOnFailure: true);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(req.TwoFactorCode))
            {
                result = await signInManager.TwoFactorAuthenticatorSignInAsync(req.TwoFactorCode, isPersistent, rememberClient: isPersistent);
            }
            else if (!string.IsNullOrEmpty(req.TwoFactorRecoveryCode))
            {
                result = await signInManager.TwoFactorRecoveryCodeSignInAsync(req.TwoFactorRecoveryCode);
            }
        }

        if (!result.Succeeded)
        {
            return Result.Unauthorized(result.ToString());
        }

        // The signInManager already produced the needed response in the form of a cookie or bearer token.

        return Result.Success();
    }
}