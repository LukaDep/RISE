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
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        Options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;
        using var initContext = new ApplicationDbContext(Options);
        initContext.Database.EnsureCreated();
    }

    public ApplicationDbContext CreateContext() => new ApplicationDbContext(Options);
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
public sealed class DbContextTransactionScope : IDisposable
{
    private readonly Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction _tx;
    public DbContextTransactionScope(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction tx) => _tx = tx;
    public void Dispose() => _tx.Rollback();
}
