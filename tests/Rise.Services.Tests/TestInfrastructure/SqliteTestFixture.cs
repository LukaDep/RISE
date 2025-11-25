
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;

namespace Rise.Services.Tests.TestInfrastructure;

public sealed class SqliteTestFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public SqliteTestFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .EnableDetailedErrors()
            .Options;

        using var ctx = new ApplicationDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    public ApplicationDbContext CreateContext()
        => new ApplicationDbContext(_options);

    public void Dispose()
    {
        _connection.Dispose();
    }
}

