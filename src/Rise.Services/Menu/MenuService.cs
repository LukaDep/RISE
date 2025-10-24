using System.Text.Json;
using System.Text.Json.Serialization;
using Rise.Shared.Common;
using Rise.Shared.Menus;
using Serilog;

namespace Rise.Services.Menus;

/// <summary>
/// Mock service voor Menu's — leest JSON bestand en geeft lijst met menu's terug.
/// </summary>
public class MockMenuService : IMenuService
{
    private readonly string _mockFilePath;

    public MockMenuService()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        // CurrentDirectory is Rise.Server → ga één niveau omhoog en dan naar Rise.Services
        _mockFilePath = Path.Combine(currentDirectory, "..", "Rise.Services", "Menu", "MockData", "MenuItemMockData.json");

        Log.Information("Current directory: {CurrentDirectory}", currentDirectory);
        Log.Information("Looking for mock file at: {MockFilePath}", _mockFilePath);
        Log.Information("File exists: {FileExists}", File.Exists(_mockFilePath));
    }

    public async Task<Result<MenuResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ct = default)
    {
        if (!File.Exists(_mockFilePath))
        {
            Log.Warning("Mock data file not found at: {MockFilePath}", _mockFilePath);
            return Result<MenuResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");
        }

        var json = await File.ReadAllTextAsync(_mockFilePath, ct);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() } // 🔸 zorgt dat "Belegdebroodje" enz. werken
        };

        var data = JsonSerializer.Deserialize<List<MenuDto.Index>>(json, options);

        if (data == null)
        {
            Log.Error("Deserialisatie van menu mockdata mislukt.");
            return Result<MenuResponse.Index>.Error("Deserialisatie mislukt");
        }

        var paged = data.Skip(request.Skip).Take(request.Take).ToList();

        var response = new MenuResponse.Index
        {
            Menus = paged
        };

        return Result.Success(response);
    }
}
