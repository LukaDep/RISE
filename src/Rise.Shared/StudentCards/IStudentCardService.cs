namespace Rise.Shared.StudentCards;

/// <summary>
/// Service interface for managing student cards.
/// </summary>
public interface IStudentCardService
{
    /// <summary>
    /// Gets the student card for the currently authenticated user.
    /// </summary>
    Task<Result<StudentCardDto>> GetByUserIdAsync(CancellationToken ct = default);
}
