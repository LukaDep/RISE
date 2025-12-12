using Microsoft.AspNetCore.Components;
using Rise.Shared.Identity.Accounts;

namespace Rise.Client.Identity;

/// <summary>
/// Code-behind for the Register page component.
/// Handles user registration with email and password.
/// </summary>
public partial class Register
{
    /// <summary>Account manager for authentication operations.</summary>
    [Inject] public required IAccountManager AccountManager { get; set; }

    private Result? _result;
    private AccountRequest.Register Model { get; set; } = new();

    /// <summary>
    /// Attempts to register a new user with the provided credentials.
    /// </summary>
    public async Task RegisterUserAsync()
    {
        _result = await AccountManager.RegisterAsync(Model.Email!, Model.Password!, Model.ConfirmPassword!);

    }
}