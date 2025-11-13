namespace Rise.Shared.StudentCards;

/// <summary>
/// Service interface for managing student cards.
/// </summary>
public interface IStudentCardService
{
    /// <summary>
    /// Gets a student card by ID.
    /// </summary>
    Task<Result<StudentCardDto>> GetStudentCardByIdAsync(Guid id, CancellationToken ct = default);
}
