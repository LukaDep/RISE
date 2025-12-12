using System.Security.Claims;
using Rise.Domain.HomeWidgets;
using Rise.Persistence;
using Rise.Services.Tests.Fakers;
using Rise.Services.Tests.TestInfrastructure;
using Rise.Services.Widgets;
using Rise.Shared.Widgets;

namespace Rise.Services.Tests.Widgets;

public class WidgetServiceShould
{
    private static ClaimsPrincipal CreateUserPrincipal(string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    private static async Task<Widget> CreateWidgetAsync(ApplicationDbContext dbContext, string typeName)
    {
        var widget = new Widget { TypeName = typeName };
        dbContext.Widgets.Add(widget);
        await dbContext.SaveChangesAsync();
        return widget;
    }

    private static async Task<UserWidget> CreateUserWidgetAsync(
        ApplicationDbContext dbContext,
        string userId,
        Widget widget,
        int x = 0,
        int y = 0,
        int width = 2,
        int height = 2,
        int minWidth = 1)
    {
        var userWidget = new UserWidget
        {
            UserId = userId,
            WidgetId = widget.Id,
            Widget = widget,
            X = x,
            Y = y,
            Width = width,
            Height = height,
            MinWidth = minWidth
        };
        dbContext.UserWidgets.Add(userWidget);
        await dbContext.SaveChangesAsync();
        return userWidget;
    }

    #region GetIndexByUserIdAsync Tests

    [Fact]
    public async Task GetIndexByUserIdAsync_WithNoUserId_ShouldReturnEmptyList()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(null!);
        var service = new WidgetService(dbContext, sessionProvider);

        // Act
        var result = await service.GetIndexByUserIdAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.UserWidgets);
    }

    [Fact]
    public async Task GetIndexByUserIdAsync_WithEmptyUserId_ShouldReturnEmptyList()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(string.Empty));
        var service = new WidgetService(dbContext, sessionProvider);

        // Act
        var result = await service.GetIndexByUserIdAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.UserWidgets);
    }

    [Fact]
    public async Task GetIndexByUserIdAsync_WithNoWidgets_ShouldReturnEmptyList()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        // Act
        var result = await service.GetIndexByUserIdAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.UserWidgets);
    }

    [Fact]
    public async Task GetIndexByUserIdAsync_WithWidgets_ShouldReturnUserWidgets()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");
        var userWidget = await CreateUserWidgetAsync(dbContext, userId, widget, x: 0, y: 0, width: 3, height: 2, minWidth: 2);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        // Act
        var result = await service.GetIndexByUserIdAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.UserWidgets);

        var returnedWidget = result.Value.UserWidgets.First();
        Assert.Equal(userWidget.Id, returnedWidget.Id);
        Assert.Equal(0, returnedWidget.X);
        Assert.Equal(0, returnedWidget.Y);
        Assert.Equal(3, returnedWidget.Width);
        Assert.Equal(2, returnedWidget.Height);
        Assert.Equal(2, returnedWidget.MinWidth);
        Assert.Equal(widget.Id, returnedWidget.Widget.Id);
        Assert.Equal("news", returnedWidget.Widget.Key);
    }

    [Fact]
    public async Task GetIndexByUserIdAsync_WithMultipleWidgets_ShouldReturnAllUserWidgets()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var newsWidget = await CreateWidgetAsync(dbContext, "news");
        var scheduleWidget = await CreateWidgetAsync(dbContext, "schedule");
        var gradesWidget = await CreateWidgetAsync(dbContext, "grades");

        await CreateUserWidgetAsync(dbContext, userId, newsWidget, x: 0, y: 0);
        await CreateUserWidgetAsync(dbContext, userId, scheduleWidget, x: 2, y: 0);
        await CreateUserWidgetAsync(dbContext, userId, gradesWidget, x: 0, y: 2);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        // Act
        var result = await service.GetIndexByUserIdAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.UserWidgets.Count());
    }

    [Fact]
    public async Task GetIndexByUserIdAsync_ShouldNotReturnDeletedWidgets()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");
        var userWidget = await CreateUserWidgetAsync(dbContext, userId, widget);

        // Mark widget as deleted
        userWidget.IsDeleted = true;
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        // Act
        var result = await service.GetIndexByUserIdAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.UserWidgets);
    }

    [Fact]
    public async Task GetIndexByUserIdAsync_ShouldNotReturnOtherUsersWidgets()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");

        await CreateUserWidgetAsync(dbContext, userId1, widget);
        await CreateUserWidgetAsync(dbContext, userId2, widget);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId1));
        var service = new WidgetService(dbContext, sessionProvider);

        // Act
        var result = await service.GetIndexByUserIdAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.UserWidgets);
    }

    #endregion

    #region UpdateUserWidgetsAsync Tests

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithEmptyRequest_ShouldRemoveAllWidgets()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");
        await CreateUserWidgetAsync(dbContext, userId, widget);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        var request = new WidgetRequest.Update { UserWidgets = new List<UserWidgetDto.Update>() };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var remaining = dbContext.UserWidgets.Where(uw => uw.UserId == userId && !uw.IsDeleted).ToList();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithNullWidgets_ShouldRemoveAllWidgets()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");
        await CreateUserWidgetAsync(dbContext, userId, widget);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        var request = new WidgetRequest.Update { UserWidgets = null };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithExistingWidget_ShouldUpdateProperties()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");
        var userWidget = await CreateUserWidgetAsync(dbContext, userId, widget, x: 0, y: 0, width: 2, height: 2, minWidth: 1);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                new()
                {
                    Id = userWidget.Id,
                    WidgetName = "news",
                    X = 5,
                    Y = 3,
                    Width = 4,
                    Height = 3,
                    MinWidth = 2
                }
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var updated = dbContext.UserWidgets.First(uw => uw.Id == userWidget.Id);
        Assert.Equal(5, updated.X);
        Assert.Equal(3, updated.Y);
        Assert.Equal(4, updated.Width);
        Assert.Equal(3, updated.Height);
        Assert.Equal(2, updated.MinWidth);
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithNewWidget_ShouldAddWidget()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        var newWidgetId = Guid.NewGuid();
        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                new()
                {
                    Id = newWidgetId,
                    WidgetName = "news",
                    X = 0,
                    Y = 0,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                }
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var added = dbContext.UserWidgets.Where(uw => uw.UserId == userId && !uw.IsDeleted).ToList();
        Assert.Single(added);
        Assert.Equal(widget.Id, added.First().WidgetId);
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithMissingWidgetInRequest_ShouldRemoveWidget()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var newsWidget = await CreateWidgetAsync(dbContext, "news");
        var scheduleWidget = await CreateWidgetAsync(dbContext, "schedule");

        var userWidget1 = await CreateUserWidgetAsync(dbContext, userId, newsWidget);
        var userWidget2 = await CreateUserWidgetAsync(dbContext, userId, scheduleWidget);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        // Only include first widget in update - second should be removed
        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                new()
                {
                    Id = userWidget1.Id,
                    WidgetName = "news",
                    X = 0,
                    Y = 0,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                }
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var remaining = dbContext.UserWidgets.Where(uw => uw.UserId == userId).ToList();
        Assert.Single(remaining);
        Assert.Equal(userWidget1.Id, remaining.First().Id);
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithNonExistentWidgetType_ShouldReturnNotFound()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    WidgetName = "nonexistent",
                    X = 0,
                    Y = 0,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                }
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Widget type(s) not found", result.Errors.First());
        Assert.Contains("nonexistent", result.Errors.First());
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithMultipleMissingWidgetTypes_ShouldReturnAllMissing()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    WidgetName = "fake1",
                    X = 0,
                    Y = 0,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    WidgetName = "fake2",
                    X = 2,
                    Y = 0,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                }
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Widget type(s) not found", result.Errors.First());
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithOtherUsersWidget_ShouldReturnForbidden()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");

        var otherUserWidget = await CreateUserWidgetAsync(dbContext, userId2, widget);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId1));
        var service = new WidgetService(dbContext, sessionProvider);

        // Try to modify another user's widget
        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                new()
                {
                    Id = otherUserWidget.Id,
                    WidgetName = "news",
                    X = 10,
                    Y = 10,
                    Width = 5,
                    Height = 5,
                    MinWidth = 3
                }
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Attempt to modify widgets not owned by the current user", result.Errors.First());
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithMixedOwnAndOtherWidgets_ShouldReturnForbidden()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");

        var ownWidget = await CreateUserWidgetAsync(dbContext, userId1, widget);
        var otherWidget = await CreateUserWidgetAsync(dbContext, userId2, widget);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId1));
        var service = new WidgetService(dbContext, sessionProvider);

        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                new()
                {
                    Id = ownWidget.Id,
                    WidgetName = "news",
                    X = 0,
                    Y = 0,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                },
                new()
                {
                    Id = otherWidget.Id,
                    WidgetName = "news",
                    X = 5,
                    Y = 5,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                }
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Attempt to modify widgets not owned by the current user", result.Errors.First());
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithDeletedWidgetId_ShouldIgnoreOwnershipCheckForDeleted()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var widget = await CreateWidgetAsync(dbContext, "news");
        var deletedWidget = await CreateUserWidgetAsync(dbContext, userId, widget);

        // Mark as deleted
        deletedWidget.IsDeleted = true;
        await dbContext.SaveChangesAsync();

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        // Try to use the deleted widget's ID in update - since it's deleted, it's treated as new
        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                new()
                {
                    Id = deletedWidget.Id,
                    WidgetName = "news",
                    X = 0,
                    Y = 0,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                }
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert - should succeed because deleted widgets are filtered out from ownership check
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_ComplexScenario_ShouldHandleAddUpdateRemove()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var newsWidget = await CreateWidgetAsync(dbContext, "news");
        var scheduleWidget = await CreateWidgetAsync(dbContext, "schedule");
        var gradesWidget = await CreateWidgetAsync(dbContext, "grades");

        var keepWidget = await CreateUserWidgetAsync(dbContext, userId, newsWidget, x: 0, y: 0);
        var removeWidget = await CreateUserWidgetAsync(dbContext, userId, scheduleWidget, x: 2, y: 0);

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                // Update existing
                new()
                {
                    Id = keepWidget.Id,
                    WidgetName = "news",
                    X = 1,
                    Y = 1,
                    Width = 3,
                    Height = 3,
                    MinWidth = 2
                },
                // Add new
                new()
                {
                    Id = Guid.NewGuid(),
                    WidgetName = "grades",
                    X = 4,
                    Y = 0,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                }
                // removeWidget is not in list, so it should be removed
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var allWidgets = dbContext.UserWidgets.Where(uw => uw.UserId == userId).ToList();
        Assert.Equal(2, allWidgets.Count);

        // Verify update
        var updated = allWidgets.First(w => w.Id == keepWidget.Id);
        Assert.Equal(1, updated.X);
        Assert.Equal(1, updated.Y);
        Assert.Equal(3, updated.Width);
        Assert.Equal(3, updated.Height);
        Assert.Equal(2, updated.MinWidth);

        // Verify new widget added
        var added = allWidgets.First(w => w.WidgetId == gradesWidget.Id);
        Assert.Equal(4, added.X);
        Assert.Equal(0, added.Y);

        // Verify removed widget is gone
        Assert.DoesNotContain(allWidgets, w => w.Id == removeWidget.Id);
    }

    [Fact]
    public async Task UpdateUserWidgetsAsync_WithCaseSensitiveWidgetName_ShouldFindWidget()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        // Widget stored with lowercase TypeName
        var widget = await CreateWidgetAsync(dbContext, "news");

        var sessionProvider = new FakeSessionContextProvider(CreateUserPrincipal(userId));
        var service = new WidgetService(dbContext, sessionProvider);

        var request = new WidgetRequest.Update
        {
            UserWidgets = new List<UserWidgetDto.Update>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    WidgetName = "news", // lowercase match
                    X = 0,
                    Y = 0,
                    Width = 2,
                    Height = 2,
                    MinWidth = 1
                }
            }
        };

        // Act
        var result = await service.UpdateUserWidgetsAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    #endregion

    #region GetCurrentUserId Edge Cases

    [Fact]
    public async Task GetIndexByUserIdAsync_WithPrincipalWithoutNameIdentifier_ShouldReturnEmptyList()
    {
        // Arrange
        using var fixture = new SqliteTestFixture();
        using var dbContext = fixture.CreateContext();

        // Create a principal without NameIdentifier claim
        var identity = new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.Email, "test@test.com")
        }, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var sessionProvider = new FakeSessionContextProvider(principal);
        var service = new WidgetService(dbContext, sessionProvider);

        // Act
        var result = await service.GetIndexByUserIdAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.UserWidgets);
    }

    #endregion
}
