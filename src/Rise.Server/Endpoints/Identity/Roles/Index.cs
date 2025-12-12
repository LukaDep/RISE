using Microsoft.EntityFrameworkCore;
using Rise.Shared.Identity;
using Microsoft.AspNetCore.Identity;

namespace Rise.Server.Endpoints.Identity.Roles;

/// <summary>
/// List all roles.
/// </summary>
/// <param name="roleManager"></param>
public class Index(RoleManager<IdentityRole> roleManager) : EndpointWithoutRequest<Result<List<KeyValuePair<string, string>>>>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Get("/api/identity/roles");
        Roles(AppRoles.Administrator);
    }

    /// <summary>
    /// Retrieves all available roles in the system.
    /// </summary>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing a list of role ID and name pairs.</returns>
    public override async Task<Result<List<KeyValuePair<string, string>>>> ExecuteAsync(CancellationToken ctx)
    {
        var roles = await roleManager.Roles.Select(r => new KeyValuePair<string, string>(r.Id, r.Name!)).ToListAsync(ctx);
        return Result.Success(roles);
    }
}