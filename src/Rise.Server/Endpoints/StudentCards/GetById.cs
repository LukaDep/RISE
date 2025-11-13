using Rise.Shared.StudentCards;

namespace Rise.Server.Endpoints.StudentCards;

/// <summary>
/// Get a student card by ID.
/// </summary>
public class GetById(IStudentCardService studentCardService) : EndpointWithoutRequest<Result<StudentCardDto>>
{
    public override void Configure()
    {
        Get("/api/studentcard/{id}");
    }

    public override async Task<Result<StudentCardDto>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        return await studentCardService.GetStudentCardByIdAsync(id, ct);
    }
}
