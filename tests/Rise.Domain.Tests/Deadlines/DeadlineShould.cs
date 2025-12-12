using Rise.Domain.Deadlines;

namespace Rise.Domain.Tests.Deadlines;

public class DeadlineShould
{
    [Fact]
    public void SetTitle_WithValidValue()
    {
        // Arrange & Act
        var deadline = new Deadline
        {
            Title = "Test Assignment",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow.AddDays(5)
        };

        // Assert
        Assert.Equal("Test Assignment", deadline.Title);
    }

    [Fact]
    public void ThrowException_WhenTitleIsNull()
    {
        // Arrange
        var deadline = new Deadline
        {
            Title = "Valid",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow
        };

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => deadline.Title = null!);
    }

    [Fact]
    public void ThrowException_WhenTitleIsEmpty()
    {
        // Arrange
        var deadline = new Deadline
        {
            Title = "Valid",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => deadline.Title = "");
    }

    [Fact]
    public void ThrowException_WhenTitleIsWhitespace()
    {
        // Arrange
        var deadline = new Deadline
        {
            Title = "Valid",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => deadline.Title = "   ");
    }

    [Fact]
    public void SetLector_WithValidValue()
    {
        // Arrange & Act
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Johnson",
            EndDate = DateTime.UtcNow.AddDays(5)
        };

        // Assert
        Assert.Equal("Prof. Johnson", deadline.Lector);
    }

    [Fact]
    public void ThrowException_WhenLectorIsNull()
    {
        // Arrange
        var deadline = new Deadline
        {
            Title = "Valid",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow
        };

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => deadline.Lector = null!);
    }

    [Fact]
    public void ThrowException_WhenLectorIsEmpty()
    {
        // Arrange
        var deadline = new Deadline
        {
            Title = "Valid",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => deadline.Lector = "");
    }

    [Fact]
    public void ConvertEndDateToUtc_WhenLocalTimeProvided()
    {
        // Arrange
        var localDate = DateTime.SpecifyKind(new DateTime(2025, 12, 25, 14, 0, 0), DateTimeKind.Local);

        // Act
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Test",
            EndDate = localDate
        };

        // Assert
        Assert.Equal(DateTimeKind.Utc, deadline.EndDate.Kind);
    }

    [Fact]
    public void KeepUtcEndDate_WhenUtcTimeProvided()
    {
        // Arrange
        var utcDate = DateTime.SpecifyKind(new DateTime(2025, 12, 25, 14, 0, 0), DateTimeKind.Utc);

        // Act
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Test",
            EndDate = utcDate
        };

        // Assert
        Assert.Equal(DateTimeKind.Utc, deadline.EndDate.Kind);
        Assert.Equal(utcDate, deadline.EndDate);
    }

    [Fact]
    public void SetDescription_WithValidValue()
    {
        // Arrange & Act
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow,
            Description = "Complete all exercises"
        };

        // Assert
        Assert.Equal("Complete all exercises", deadline.Description);
    }

    [Fact]
    public void AllowNullDescription()
    {
        // Arrange & Act
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow,
            Description = null
        };

        // Assert
        Assert.Null(deadline.Description);
    }

    [Fact]
    public void SetCourse_WithValidValue()
    {
        // Arrange & Act
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow,
            Course = "Mathematics 101"
        };

        // Assert
        Assert.Equal("Mathematics 101", deadline.Course);
    }

    [Fact]
    public void AllowNullCourse()
    {
        // Arrange & Act
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow,
            Course = null
        };

        // Assert
        Assert.Null(deadline.Course);
    }

    [Fact]
    public void SetUserId_WithValidValue()
    {
        // Arrange & Act
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow,
            UserId = "user-123"
        };

        // Assert
        Assert.Equal("user-123", deadline.UserId);
    }

    [Fact]
    public void AllowNullUserId()
    {
        // Arrange & Act
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow,
            UserId = null
        };

        // Assert
        Assert.Null(deadline.UserId);
    }

    [Fact]
    public void UpdateTitle_AfterCreation()
    {
        // Arrange
        var deadline = new Deadline
        {
            Title = "Original Title",
            Lector = "Prof. Test",
            EndDate = DateTime.UtcNow
        };

        // Act
        deadline.Title = "Updated Title";

        // Assert
        Assert.Equal("Updated Title", deadline.Title);
    }

    [Fact]
    public void UpdateLector_AfterCreation()
    {
        // Arrange
        var deadline = new Deadline
        {
            Title = "Test",
            Lector = "Prof. Original",
            EndDate = DateTime.UtcNow
        };

        // Act
        deadline.Lector = "Prof. Updated";

        // Assert
        Assert.Equal("Prof. Updated", deadline.Lector);
    }
}
