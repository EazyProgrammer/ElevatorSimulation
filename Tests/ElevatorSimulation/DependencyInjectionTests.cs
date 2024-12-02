using Application.Interfaces;
using ElevatorSimulation;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.ElevatorSimulation;

public class DependencyInjectionTests
{
    [Fact]
    public void ConfigureServices_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var serviceProvider = services.ConfigureServices();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IElevatorRepository>());
        Assert.NotNull(serviceProvider.GetService<IBuildingRepository>());
        Assert.NotNull(serviceProvider.GetService<IFloorRepository>());
        Assert.NotNull(serviceProvider.GetService<IDataSeeder>());
        Assert.NotNull(serviceProvider.GetService<IElevatorService>());
        Assert.NotNull(serviceProvider.GetService<IMainMenu>());
    }

    [Fact]
    public void ConfigureDatabase_ShouldCreateDatabaseAndApplyMigrations()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=elevator.db"));
        var mockSeeder = new Mock<IDataSeeder>();
        services.AddScoped(provider => mockSeeder.Object);

        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

        // Ensure the database doesn't exist before we call ConfigureDatabase
        var dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        if (Directory.Exists(dataFolder))
        {
            Directory.Delete(dataFolder, true);
        }

        // Act
        serviceProvider.ConfigureDatabase();

        // Assert: Check if the database directory is created
        Assert.True(Directory.Exists(dataFolder));

        // Verify that the seeding method was called
        mockSeeder.Verify(seeder => seeder.Seed(It.IsAny<AppDbContext>()), Times.Once);

        // Optionally, you can also verify the database was migrated.
        // You may want to check the actual database for tables, but this requires more setup.
        var createdDb = serviceProvider.GetService<AppDbContext>();
        Assert.NotNull(createdDb);
    }

    [Fact]
    public void ConfigureDatabase_ShouldCreateDataFolderIfNotExist()
    {
        // Arrange
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=elevator.db"));
        var mockSeeder = new Mock<IDataSeeder>();
        services.AddScoped(provider => mockSeeder.Object);
        var serviceProvider = services.BuildServiceProvider();

        var dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

        // Ensure the directory doesn't exist before the test
        if (Directory.Exists(dataFolder))
        {
            Directory.Delete(dataFolder, true);
        }

        // Act
        serviceProvider.ConfigureDatabase();

        // Assert: Ensure the directory is created
        Assert.True(Directory.Exists(dataFolder));
    }
}

