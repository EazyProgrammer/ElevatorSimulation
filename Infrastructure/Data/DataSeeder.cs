using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;

namespace Infrastructure.Data;

public class DataSeeder : IDataSeeder
{
    public async Task Seed(AppDbContext context)
    {
        if (!context.Buildings.Any())
        {
            var building = new Building
            {
                Name = "Main Office Building",
                TotalFloors = 10
            };

            for (int i = 0; i <= building.TotalFloors; i++)
            {
                building.Floors.Add(new Floor
                {
                    FloorNumber = i,
                    WaitingPassengers = 0
                });
            }

            building.Elevators.Add(new Elevator
            {
                Name = DefaultElevatorName.ElevatorA,
                CurrentFloor = 0,
                MaxCapacity = 10,
                Direction = Direction.Stationary
            });

            building.Elevators.Add(new Elevator
            {
                Name = DefaultElevatorName.ElevatorB,
                CurrentFloor = 0,
                MaxCapacity = 10,
                Direction = Direction.Stationary
            });

            context.Buildings.Add(building);
            await context.SaveChangesAsync();
        }
    }
}
