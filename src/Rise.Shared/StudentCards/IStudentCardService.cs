namespace Rise.Shared.StudentCards;

/// <summary>
/// Service interface for managing student cards.
/// </summary>
public interface IStudentCardService
{
    /// <summary>
    /// Retrieves the student card for the current user.
    /// Determines if the card is still valid based on the expiration date.
    /// </summary>
    /// <param name="ct">CancellationToken to cancel the operation</param>
    /// <returns>Result with StudentCardDto containing card details, or NotFound if no card found</returns>
    Task<Result<StudentCardDto>> GetByUserIdAsync(CancellationToken ct = default);
}
