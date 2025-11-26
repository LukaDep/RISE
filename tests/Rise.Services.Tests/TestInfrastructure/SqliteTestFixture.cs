using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;


namespace Rise.Services.Tests.TestInfrastructure;

public class SqliteTestFixture : IDisposable
{
    private readonly DbConnection _connection;

    public DbContextOptions<ApplicationDbContext> Options { get; }

    public SqliteTestFixture()
    {
        // Use an in-memory SQLite database. Keeping the connection open preserves schema & data.
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        Options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        // Create schema from current model
        // Note: This requires migrations to be compatible with SQLite (TEXT instead of json)
        using var initContext = new ApplicationDbContext(Options);
        initContext.Database.EnsureCreated();
    }

    public ApplicationDbContext CreateContext() => new ApplicationDbContext(Options);

    /// <summary>
    /// Creates a context wrapped in a transaction that will be rolled back when disposed.
    /// Useful for test-level isolation without recreating the database.
    /// </summary>
    public (ApplicationDbContext Context, DbContextTransactionScope Scope) CreateTransactionalContext()
    {
        var ctx = CreateContext();
        var scope = new DbContextTransactionScope(ctx.Database.BeginTransaction());
        return (ctx, scope);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}

/// <summary>
/// Helper wrapper to dispose and rollback transaction.
/// </summary>
public sealed class DbContextTransactionScope : IDisposable
{
    private readonly Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction _tx;
    public DbContextTransactionScope(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction tx) => _tx = tx;
    public void Dispose() => _tx.Rollback();
}
