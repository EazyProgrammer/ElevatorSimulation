using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure;

public class AppDbContextFactoryTests
{
    [Fact]
    public void CreateDbContext_ShouldReturnNonNullAppDbContext()
    {
        // Arrange
        var factory = new AppDbContextFactory();

        // Act
        var dbContext = factory.CreateDbContext(new string[] { });

        // Assert
        Assert.NotNull(dbContext);
        Assert.IsType<AppDbContext>(dbContext);  // Verify it's an instance of AppDbContext
    }

    [Fact]
    public void CreateDbContext_ShouldUseSqliteConnectionString()
    {
        // Arrange
        var factory = new AppDbContextFactory();

        // Act
        var dbContext = factory.CreateDbContext(new string[] { });

        // Assert
        var options = dbContext.Database.GetDbConnection().ConnectionString;
        Assert.Equal("Data Source=elevator.db", options);  // Ensure the connection string is correct
    }

    [Fact]
    public void CreateDbContext_ShouldInitializeDbContextWithSqliteProvider()
    {
        // Arrange
        var factory = new AppDbContextFactory();

        // Act
        var dbContext = factory.CreateDbContext(new string[] { });

        // Assert
        Assert.Contains("Microsoft.EntityFrameworkCore.Sqlite", dbContext.Database.ProviderName);  // Verify SQLite provider is used
    }
}
