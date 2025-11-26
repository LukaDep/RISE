using System.Net.Http.Json;
using Rise.Shared.Identity.Accounts;
using Rise.Shared.StudentCards;

namespace Rise.Client.Account;

public class StudentCardService(HttpClient httpClient) : IStudentCardService
{
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
