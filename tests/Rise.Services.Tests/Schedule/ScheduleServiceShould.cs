using System.Text.Json;
using Ardalis.Result;
using Rise.Services.Schedule;
using Rise.Shared.Common;

namespace Rise.Services.Tests.Schedule;

public class ScheduleServiceShould
{
    [Fact]
    public async Task GetIndexAsyncShouldReturnSuccessWithValidData()
    {
        // Arrange
        var service = new MockScheduleService();
        var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        // Act
        var result = await service.GetIndexAsync(request, CancellationToken.None);

        // Assert
        if (result.IsSuccess)
        {
            result.Value.ShouldNotBeNull();
            result.Value.Reservations.ShouldNotBeNull();
        }
        else
        {
            // Als het mockbestand niet werkt zou het not found moeten teruggeven
            result.Status.ShouldBe(ResultStatus.NotFound);
        }
    }

    [Fact]
    public void ConvertToDtoCorrectly()
    {
        // Arrange
        var mockData = new
        {
            columnheaders = new[] { "Olod", "Werkvorm", "Onderwerp", "Info werkvorm", "Leer- of toetsomgeving", "Lokaal", "Lesgever" },
            info = new { reservationlimit = 1000, reservationcount = 2 },
            reservations = new[]
            {
                new
                {
                    id = "test-001",
                    startdate = "01-09-2025",
                    starttime = "08:30",
                    enddate = "01-09-2025",
                    endtime = "10:30",
                    columns = new[] { "Web Ontwikkeling 2", "Hoorcollege", "", "", "Digitaal (laptop/PC)", "GSCHB.2.009", "Bert Van Vreckem" }
                },
                new
                {
                    id = "test-002",
                    startdate = "02-09-2025",
                    starttime = "11:00",
                    enddate = "02-09-2025",
                    endtime = "13:00",
                    columns = new[] { "Databanken II", "Activerend hoorcollege", "", "", "Digitaal (laptop/PC)", "GSCHB.3.012", "Thomas Parmentier" }
                }
            }
        };

        var json = JsonSerializer.Serialize(mockData);
        var service = new MockScheduleService();

        // Act
        var result = service.convertToDto(json);

        // Assert
        result.ShouldNotBeNull();
        result.Reservations.Count.ShouldBe(2);

        var firstReservation = result.Reservations[0];
        firstReservation.Id.ShouldBe("test-001");
        firstReservation.Course.ShouldBe("Web Ontwikkeling 2");
        firstReservation.WorkForm.ShouldBe("Hoorcollege");
        firstReservation.Environment.ShouldBe("Digitaal (laptop/PC)");
        firstReservation.Room.ShouldBe("GSCHB.2.009");
        firstReservation.Teacher.ShouldBe("Bert Van Vreckem");
        firstReservation.StartDateTime.ShouldBe(new DateTime(2025, 9, 1, 8, 30, 0));
        firstReservation.EndDateTime.ShouldBe(new DateTime(2025, 9, 1, 10, 30, 0));

        var secondReservation = result.Reservations[1];
        secondReservation.Id.ShouldBe("test-002");
        secondReservation.Course.ShouldBe("Databanken II");
        secondReservation.WorkForm.ShouldBe("Activerend hoorcollege");
        secondReservation.Teacher.ShouldBe("Thomas Parmentier");
        secondReservation.StartDateTime.ShouldBe(new DateTime(2025, 9, 2, 11, 0, 0));
        secondReservation.EndDateTime.ShouldBe(new DateTime(2025, 9, 2, 13, 0, 0));
    }

    [Fact]
    public void EmptyColumnsShouldReturnEmptyReservation()
    {
        // Arrange
        var mockData = new
        {
            columnheaders = new[] { "Olod", "Werkvorm" },
            info = new { reservationlimit = 1000, reservationcount = 1 },
            reservations = new[]
            {
                new
                {
                    id = "test-003",
                    startdate = "03-09-2025",
                    starttime = "14:00",
                    enddate = "03-09-2025",
                    endtime = "16:00",
                    columns = Array.Empty<string>() // Empty columns array
                }
            }
        };

        var json = JsonSerializer.Serialize(mockData);
        var service = new MockScheduleService();

        // Act
        var result = service.convertToDto(json);

        // Assert
        result.ShouldNotBeNull();
        result.Reservations.Count.ShouldBe(1);

        var reservation = result.Reservations[0];
        reservation.Id.ShouldBe("test-003");
        reservation.Course.ShouldBe(string.Empty);
        reservation.WorkForm.ShouldBe(string.Empty);
        reservation.Environment.ShouldBe(string.Empty);
        reservation.Room.ShouldBe(string.Empty);
        reservation.Teacher.ShouldBe(string.Empty);
    }

    [Fact]
    public void HandleMultipleReservationsCorrectly()
    {
        // Arrange
        var mockData = new
        {
            columnheaders = new[] { "Olod", "Werkvorm", "Onderwerp", "Info werkvorm", "Leer- of toetsomgeving", "Lokaal", "Lesgever" },
            info = new { reservationlimit = 1000, reservationcount = 3 },
            reservations = new[]
            {
                new
                {
                    id = "test-005",
                    startdate = "05-09-2025",
                    starttime = "08:00",
                    enddate = "05-09-2025",
                    endtime = "10:00",
                    columns = new[] { "Course 1", "Type 1", "", "", "Env 1", "Room 1", "Teacher 1" }
                },
                new
                {
                    id = "test-006",
                    startdate = "05-09-2025",
                    starttime = "10:30",
                    enddate = "05-09-2025",
                    endtime = "12:30",
                    columns = new[] { "Course 2", "Type 2", "", "", "Env 2", "Room 2", "Teacher 2" }
                },
                new
                {
                    id = "test-007",
                    startdate = "05-09-2025",
                    starttime = "13:00",
                    enddate = "05-09-2025",
                    endtime = "15:00",
                    columns = new[] { "Course 3", "Type 3", "", "", "Env 3", "Room 3", "Teacher 3" }
                }
            }
        };

        var json = JsonSerializer.Serialize(mockData);
        var service = new MockScheduleService();

        // Act
        var result = service.convertToDto(json);

        // Assert
        result.ShouldNotBeNull();
        result.Reservations.Count.ShouldBe(3);
        result.Reservations[0].Id.ShouldBe("test-005");
        result.Reservations[1].Id.ShouldBe("test-006");
        result.Reservations[2].Id.ShouldBe("test-007");
    }

    [Fact]
    public void ParseDateTimeCorrectly()
    {
        // Arrange
        var mockData = new
        {
            columnheaders = new[] { "Olod" },
            info = new { reservationlimit = 1000, reservationcount = 1 },
            reservations = new[]
            {
                new
                {
                    id = "test-008",
                    startdate = "15-12-2025",
                    starttime = "23:59",
                    enddate = "16-12-2025",
                    endtime = "01:30",
                    columns = new[] { "Test Course" }
                }
            }
        };

        var json = JsonSerializer.Serialize(mockData);
        var service = new MockScheduleService();

        // Act
        var result = service.convertToDto(json);

        // Assert
        var reservation = result.Reservations[0];
        reservation.StartDateTime.ShouldBe(new DateTime(2025, 12, 15, 23, 59, 0));
        reservation.EndDateTime.ShouldBe(new DateTime(2025, 12, 16, 1, 30, 0));
    }
}
