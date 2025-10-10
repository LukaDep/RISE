using Rise.Shared.TimeEdit;

namespace Rise.Server.Endpoints.Schedule;

/// <summary>
/// List all products.
/// See https://fast-endpoints.com/
/// </summary>
/// <param name="productService"></param>
public class Index(ITimeEditService timeEditService) : Endpoint<TimeEditRequest.Get, Result<TimeEditDto.ApiResponse>>
{
  public override void Configure()
  {
    Get("/api/schedule");
    AllowAnonymous();
  }

  public override Task<Result<TimeEditDto.ApiResponse>> ExecuteAsync(TimeEditRequest.Get req, CancellationToken ct)
  {
    return timeEditService.GetAsync(req, ct);
  }
}