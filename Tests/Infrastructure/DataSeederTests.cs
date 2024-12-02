using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure;

public class DataSeederTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "ElevatorSimulationTestDb")
            .Options;
    }

    private AppDbContext GetDbContext()
    {
        return new AppDbContext(GetInMemoryDatabaseOptions());
    }

    [Fact]
    public async Task Seed_ShouldAddBuilding_WhenBuildingsTableIsEmpty()
    {
        // Arrange
        var context = GetDbContext();
        var dataSeeder = new DataSeeder();

        // Act
        await dataSeeder.Seed(context);

        // Assert
        var building = context.Buildings.FirstOrDefault();
        Assert.NotNull(building);

        // Verify that the building has floors and elevators
        var elevatorA = building.Elevators.FirstOrDefault(e => e.Name == DefaultElevatorName.ElevatorA);
        var elevatorB = building.Elevators.FirstOrDefault(e => e.Name == DefaultElevatorName.ElevatorB);
        Assert.NotNull(elevatorA);
        Assert.NotNull(elevatorB);
        Assert.Equal(0, elevatorA.CurrentFloor);
        Assert.Equal(0, elevatorB.CurrentFloor);
    }

    [Fact]
    public async Task Seed_ShouldNotAddBuilding_WhenBuildingsTableIsNotEmpty()
    {
        // Arrange
        var context = GetDbContext();
        var dataSeeder = new DataSeeder();

        // Pre-populate with a building
        context.Buildings.Add(new Building
        {
            Name = "Existing Building",
            TotalFloors = 5,
            Floors = new List<Floor> { new Floor { FloorNumber = 1 } },
            Elevators = new List<Elevator>
            {
                new Elevator { Name = "Elevator C", CurrentFloor = 0, MaxCapacity = 10, Direction = Direction.Stationary }
            }
        });
        await context.SaveChangesAsync();

        // Act
        await dataSeeder.Seed(context);

        // Assert: Ensure only one building exists, and it's the "Existing Building"
        var buildings = context.Buildings.ToList();
        Assert.Single(buildings); // Only one building should exist
        Assert.Equal("Existing Building", buildings.First().Name);
    }
}
