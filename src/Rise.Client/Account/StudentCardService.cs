using System.Net.Http.Json;
using Rise.Shared.Identity.Accounts;
using Rise.Shared.StudentCards;

namespace Rise.Client.Account;

/// <summary>
/// Client-side service for retrieving student card information.
/// </summary>
/// <param name="httpClient">The HTTP client for API communication.</param>
public class StudentCardService(HttpClient httpClient) : IStudentCardService
{
    /// <summary>
    /// Retrieves the student card for the current user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the student card data, or not found if unavailable.</returns>
    public async Task<Result<StudentCardDto>> GetByUserIdAsync(CancellationToken ct = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<AccountResponse.Info>>("api/identity/accounts/info", cancellationToken: ct);
        if (result == null)
            return Result<StudentCardDto>.NotFound("Failed to retrieve account information.");
        if (result.Value.StudentCard == null)
            return Result<StudentCardDto>.NotFound();
        return Result<StudentCardDto>.Success(result.Value.StudentCard);
    }
}
