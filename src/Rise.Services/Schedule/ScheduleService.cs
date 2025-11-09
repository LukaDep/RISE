using System.Text.Json;
using Rise.Persistence;
using Rise.Services.Absences;
using Rise.Shared.Absences;
using Rise.Shared.Common;
using Rise.Shared.Schedule;
using Serilog;

namespace Rise.Services.Schedule;

public class MockScheduleService(ApplicationDbContext dbContext) : IScheduleService
{
    private string? _mockFilePath;

    private AbsencesService _absencesService = new AbsencesService(dbContext);

    

    public async Task<Result<ScheduleDto.Data>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ct)
    {
        // Pad naar het JSON-bestand in de source code directory
        var currentDirectory = Directory.GetCurrentDirectory();
        // CurrentDirectory is Rise.Server, dus we gaan een level omhoog en dan naar Rise.Services
        _mockFilePath = Path.Combine(currentDirectory, "..", "Rise.Services", "Schedule", "MockData", "ScheduleMockdata.json");
        Log.Information("Current directory: {CurrentDirectory}", currentDirectory);
        Log.Information("Looking for mock file at: {MockFilePath}", _mockFilePath);
        Log.Information("File exists: {FileExists}", File.Exists(_mockFilePath));
        if (!File.Exists(_mockFilePath))
        {
            Log.Warning("Mock data file not found at: {MockFilePath}", _mockFilePath);
            return Result<ScheduleDto.Data>.NotFound($"Mock data file not found at: {_mockFilePath}");
        }

        var json = await File.ReadAllTextAsync(_mockFilePath, ct);


        var data = ConvertToDto(json);

        //get absences data
        var absencesResult = await _absencesService.GetIndexAsync(new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 100,
        }, ct);

        var absences = absencesResult?.Value?.Absences ?? Enumerable.Empty<AbsenceDto.Index>();

        data.Reservations.ForEach(r =>
        {
            r.IsAbsent = absences.Any(a =>
                string.Equals(a.Name, r.Teacher, StringComparison.OrdinalIgnoreCase)
                && r.StartDateTime.Date >= a.StartDate.Date
                && r.StartDateTime.Date <= a.EndDate.Date
            );
        });

        if (data == null)
            return Result<ScheduleDto.Data>.Error("Deserialisatie mislukt");

        return Result.Success(data);
    }

    public static ScheduleDto.Data ConvertToDto(string json)
    {
        // Deserialize the raw API response
        var rawData = JsonSerializer.Deserialize<ScheduleApiResponse.ScheduleData>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (rawData == null)
            throw new InvalidOperationException("Deserialization of ScheduleApiResponse.ScheduleData returned null.");

        // Convert raw reservations with Columns array to DTO with named properties
        var convertedReservations = rawData.Reservations.Select(r => new ScheduleDto.Reservation
        {
            Id = r.Id,
            StartDateTime = ParseDateTime(r.StartDate, r.StartTime),
            EndDateTime = ParseDateTime(r.EndDate, r.EndTime),
            // Map columns array to named properties based on the structure:
            // [0] = Olod (Course), [1] = Werkvorm, [4] = Leer- of toetsomgeving, [5] = Lokaal, [6] = Lesgever
            Course = r.Columns.Count > 0 ? r.Columns[0] : string.Empty,
            WorkForm = r.Columns.Count > 1 ? r.Columns[1] : string.Empty,
            Environment = r.Columns.Count > 4 ? r.Columns[4] : string.Empty,
            Room = r.Columns.Count > 5 ? r.Columns[5] : string.Empty,
            Teacher = r.Columns.Count > 6 ? r.Columns[6] : string.Empty
        }).ToList();

        return new ScheduleDto.Data
        {
            Reservations = convertedReservations
        };
    }

    /// <summary>
    /// Parses date and time strings into a DateTime object.
    /// Expected format: date = "dd-MM-yyyy", time = "HH:mm"
    /// </summary>
    private static DateTime ParseDateTime(string date, string time)
    {
        var dateParts = date.Split('-');
        var timeParts = time.Split(':');

        if (dateParts.Length == 3 && timeParts.Length == 2 &&
            int.TryParse(dateParts[0], out int day) &&
            int.TryParse(dateParts[1], out int month) &&
            int.TryParse(dateParts[2], out int year) &&
            int.TryParse(timeParts[0], out int hour) &&
            int.TryParse(timeParts[1], out int minute))
        {
            return new DateTime(year, month, day, hour, minute, 0);
        }

        throw new FormatException($"Invalid date/time format: {date} {time}");
    }
}

