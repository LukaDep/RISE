using Rise.Domain.Events;

namespace Rise.Domain.Tests.Events;

public class EventShould
{
    [Fact]
    public void SetTitle_WithValidValue()
    {
        // Arrange & Act
        var ev = new Event
        {
            Title = "Basketball Game",
            Location = "Sports Hall",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow.AddDays(1),
            EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(2)
        };

        // Assert
        Assert.Equal("Basketball Game", ev.Title);
    }

    [Fact]
    public void ThrowException_WhenTitleIsNull()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Valid",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => ev.Title = null!);
    }

    [Fact]
    public void ThrowException_WhenTitleIsEmpty()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Valid",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ev.Title = "");
    }

    [Fact]
    public void ThrowException_WhenTitleIsWhitespace()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Valid",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ev.Title = "   ");
    }

    [Fact]
    public void SetLocation_WithValidValue()
    {
        // Arrange & Act
        var ev = new Event
        {
            Title = "Test Event",
            Location = "Auditorium A",
            Type = "Academic",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Assert
        Assert.Equal("Auditorium A", ev.Location);
    }

    [Fact]
    public void ThrowException_WhenLocationIsNull()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Test",
            Location = "Valid",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => ev.Location = null!);
    }

    [Fact]
    public void ThrowException_WhenLocationIsEmpty()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Test",
            Location = "Valid",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ev.Location = "");
    }

    [Fact]
    public void SetType_WithValidValue()
    {
        // Arrange & Act
        var ev = new Event
        {
            Title = "Yoga Session",
            Location = "Wellness Center",
            Type = "Welzijn",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Assert
        Assert.Equal("Welzijn", ev.Type);
    }

    [Fact]
    public void ThrowException_WhenTypeIsNull()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Valid",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => ev.Type = null!);
    }

    [Fact]
    public void ThrowException_WhenTypeIsEmpty()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Valid",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ev.Type = "");
    }

    [Fact]
    public void ConvertStartDateTimeToUtc_WhenLocalTimeProvided()
    {
        // Arrange
        var localDate = DateTime.SpecifyKind(new DateTime(2025, 12, 25, 14, 0, 0), DateTimeKind.Local);

        // Act
        var ev = new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Sport",
            StartDateTime = localDate,
            EndDateTime = DateTime.UtcNow.AddHours(2)
        };

        // Assert
        Assert.Equal(DateTimeKind.Utc, ev.StartDateTime.Kind);
    }

    [Fact]
    public void KeepUtcStartDateTime_WhenUtcTimeProvided()
    {
        // Arrange
        var utcDate = DateTime.SpecifyKind(new DateTime(2025, 12, 25, 14, 0, 0), DateTimeKind.Utc);

        // Act
        var ev = new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Sport",
            StartDateTime = utcDate,
            EndDateTime = DateTime.UtcNow.AddHours(2)
        };

        // Assert
        Assert.Equal(DateTimeKind.Utc, ev.StartDateTime.Kind);
        Assert.Equal(utcDate, ev.StartDateTime);
    }

    [Fact]
    public void ConvertEndDateTimeToUtc_WhenLocalTimeProvided()
    {
        // Arrange
        var localDate = DateTime.SpecifyKind(new DateTime(2025, 12, 25, 16, 0, 0), DateTimeKind.Local);

        // Act
        var ev = new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = localDate
        };

        // Assert
        Assert.Equal(DateTimeKind.Utc, ev.EndDateTime.Kind);
    }

    [Fact]
    public void KeepUtcEndDateTime_WhenUtcTimeProvided()
    {
        // Arrange
        var utcDate = DateTime.SpecifyKind(new DateTime(2025, 12, 25, 16, 0, 0), DateTimeKind.Utc);

        // Act
        var ev = new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = utcDate
        };

        // Assert
        Assert.Equal(DateTimeKind.Utc, ev.EndDateTime.Kind);
        Assert.Equal(utcDate, ev.EndDateTime);
    }

    [Fact]
    public void ThrowException_WhenStartDateTimeIsDefault()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Sport",
            StartDateTime = default,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        });
    }

    [Fact]
    public void ThrowException_WhenEndDateTimeIsDefault()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = default
        });
    }

    [Fact]
    public void SetDescription_WithValidValue()
    {
        // Arrange & Act
        var ev = new Event
        {
            Title = "Workshop",
            Location = "Room B",
            Type = "Academic",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(2),
            Description = "Learn new skills"
        };

        // Assert
        Assert.Equal("Learn new skills", ev.Description);
    }

    [Fact]
    public void AllowNullDescription()
    {
        // Arrange & Act
        var ev = new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1),
            Description = null
        };

        // Assert
        Assert.Null(ev.Description);
    }

    [Fact]
    public void SetRegistrationLink_WithValidValue()
    {
        // Arrange & Act
        var ev = new Event
        {
            Title = "Conference",
            Location = "Hall A",
            Type = "Academic",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(3),
            RegistrationLink = "https://example.com/register"
        };

        // Assert
        Assert.Equal("https://example.com/register", ev.RegistrationLink);
    }

    [Fact]
    public void AllowNullRegistrationLink()
    {
        // Arrange & Act
        var ev = new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1),
            RegistrationLink = null
        };

        // Assert
        Assert.Null(ev.RegistrationLink);
    }

    [Fact]
    public void UpdateTitle_AfterCreation()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Original",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        ev.Title = "Updated Title";

        // Assert
        Assert.Equal("Updated Title", ev.Title);
    }

    [Fact]
    public void UpdateLocation_AfterCreation()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Test",
            Location = "Original Location",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        ev.Location = "New Location";

        // Assert
        Assert.Equal("New Location", ev.Location);
    }

    [Fact]
    public void UpdateType_AfterCreation()
    {
        // Arrange
        var ev = new Event
        {
            Title = "Test",
            Location = "Test",
            Type = "Sport",
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        ev.Type = "Welzijn";

        // Assert
        Assert.Equal("Welzijn", ev.Type);
    }
}
