using Rise.Shared.Identity;
using Rise.Shared.Identity.Roles;
using Microsoft.AspNetCore.Identity;

namespace Rise.Server.Endpoints.Identity.Roles;

/// <summary>
/// Create a new role.
/// </summary>
/// <param name="roleManager"></param>
public class Create(RoleManager<IdentityRole> roleManager) : Endpoint<RoleRequest.Create, Result<string>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Post("/api/identity/roles");
        Roles(AppRoles.Administrator);
    }

    /// <summary>
    /// Creates a new role in the system.
    /// </summary>
    /// <param name="req">The create request containing the role name.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the new role's ID, or an error if creation failed.</returns>
    public override async Task<Result<string>> ExecuteAsync(RoleRequest.Create req, CancellationToken ctx)
    {
        if (await roleManager.RoleExistsAsync(req.Name))
            return Result.Conflict($"Role with name '{req.Name}' already exists.");

        IdentityRole role = new()
        {
            Name = req.Name,
            NormalizedName = req.Name.ToUpper()
        };

        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
            return Result.Error(result.Errors.First().Description);

        return Result.Created(role.Id);
    }
}