using System.Text.Json;
using Rise.Shared.TimeEdit;
using Serilog;

namespace Rise.Services.TimeEdit;

public class MockTimeEditService : ITimeEditService
{
  private readonly string _mockFilePath;

  public MockTimeEditService()
  {
    // Pad naar het JSON-bestand in de source code directory
    var currentDirectory = Directory.GetCurrentDirectory();
    // CurrentDirectory is Rise.Server, dus we gaan een level omhoog en dan naar Rise.Services
    _mockFilePath = Path.Combine(currentDirectory, "..", "Rise.Services", "TimeEdit", "mock", "TimeEditMockdata.json");
    Log.Information("Current directory: {CurrentDirectory}", currentDirectory);
    Log.Information("Looking for mock file at: {MockFilePath}", _mockFilePath);
    Log.Information("File exists: {FileExists}", File.Exists(_mockFilePath));
  }

  public async Task<Result<TimeEditDto.ApiResponse>> GetAsync(TimeEditRequest.Get req, CancellationToken ct)
  {
    if (!File.Exists(_mockFilePath))
    {
      Log.Warning("Mock data file not found at: {MockFilePath}", _mockFilePath);
      return Result<TimeEditDto.ApiResponse>.NotFound($"Mock data file not found at: {_mockFilePath}");
    }

    var json = await File.ReadAllTextAsync(_mockFilePath, ct);
    var data = JsonSerializer.Deserialize<TimeEditDto.ApiResponse>(json, new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    });

    if (data == null)
      return Result<TimeEditDto.ApiResponse>.Error("Deserialisatie mislukt");

    return Result.Success(data);
  }
}
