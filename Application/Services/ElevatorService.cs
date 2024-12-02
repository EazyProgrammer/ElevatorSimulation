using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Interfaces;

namespace Application.Services;

public class ElevatorService : IElevatorService
{
    private readonly IElevatorRepository _elevatorRepository;
    private readonly IFloorRepository _floorRepository;

    public ElevatorService(IElevatorRepository elevatorRepository, IFloorRepository floorRepository)
    {
        _elevatorRepository = elevatorRepository;
        _floorRepository = floorRepository;
    }

    public async Task<IEnumerable<ElevatorStatus>> GetElevatorStatusesAsync()
    {
        var elevators = await _elevatorRepository.GetAllAsync();
        return elevators.Select(e => new ElevatorStatus
        {
            ElevatorName = e.Name,
            CurrentFloor = e.CurrentFloor,
            Direction = e.Direction,
            IsInMotion = e.IsInMotion,
            CurrentLoad = e.CurrentLoad,
            MaxCapacity = e.MaxCapacity
        });
    }

    public async Task CallElevatorAsync(int floor, int passengers)
    {
        var elevators = await _elevatorRepository.GetAllAsync();
        var nearestElevator = elevators
            .Where(e => e.CurrentLoad + passengers <= e.MaxCapacity && !e.IsInMotion)
            .OrderBy(e => Math.Abs(e.CurrentFloor - floor))
            .FirstOrDefault();

        if (nearestElevator == null)
        {
            Console.WriteLine("No available elevator. Please wait.");
            return;
        }

        nearestElevator.IsInMotion = true;
        nearestElevator.Direction = nearestElevator.CurrentFloor < floor ? Direction.Up : Direction.Down;
        nearestElevator.TargetFloors.Add(floor);

        await SimulateMovementAsync(nearestElevator);

        nearestElevator.CurrentLoad += passengers;
        nearestElevator.IsInMotion = false;
        nearestElevator.Direction = Direction.Stationary;
        nearestElevator.CurrentLoad = 0;

        await _elevatorRepository.UpdateAsync(nearestElevator);
    }

    public async Task SimulateMovementAsync(Elevator elevator)
    {
        Console.WriteLine();

        while (elevator.TargetFloors.Any())
        {
            var nextFloor = elevator.TargetFloors.First();

            if (elevator.CurrentFloor < nextFloor)
            {
                elevator.CurrentFloor++;
                elevator.Direction = Direction.Up;
            }
            else if (elevator.CurrentFloor > nextFloor)
            {
                elevator.CurrentFloor--;
                elevator.Direction = Direction.Down;
            }

            if (elevator.CurrentFloor == nextFloor)
            {
                // Elevator has reached the target floor
                Console.WriteLine($"{elevator.Name} arrived at floor {elevator.CurrentFloor}.");

                // Simulate passengers leaving
                var passengersExiting = SimulatePassengersExiting(elevator);
                elevator.CurrentLoad -= passengersExiting;
                Console.WriteLine($"{passengersExiting} passengers exited. Current load: {elevator.CurrentLoad}");

                // Remove the floor from target list
                elevator.TargetFloors.Remove(nextFloor);

                // Clear direction if no more target floors
                if (!elevator.TargetFloors.Any())
                {
                    elevator.Direction = Direction.Stationary;
                }
            }

            // Simulate real-time movement
            Console.WriteLine($"{elevator.Name} at floor {elevator.CurrentFloor}, Status: {elevator.Direction}");

            // Persist elevator state during movement
            await _elevatorRepository.UpdateAsync(elevator);

            await Task.Delay(3000); // Simulate 1 second per floor
        }

        Console.WriteLine($"{elevator.Name} has completed all requests.");
        elevator.IsInMotion = false;
        await _elevatorRepository.UpdateAsync(elevator);

        Console.WriteLine();
    }

    private int SimulatePassengersExiting(Elevator elevator)
    {
        // Logic to determine how many passengers exit
        // For simplicity, assume a random number between 0 and half the current load
        var random = new Random();
        return random.Next(0, elevator.CurrentLoad / 2 + 1);
    }

    public async Task CreateElevatorAsync()
    {
        // Get validated elevator details from user
        var name = await GetElevatorNameAsync();
        if (name == null) return;

        var maxCapacity = await GetMaxCapacityAsync();
        if (maxCapacity == null) return;

        var buildingId = await GetBuildingIdAsync();
        if (buildingId == null) return;

        var elevator = new Elevator
        {
            Name = name,
            MaxCapacity = maxCapacity.Value,
            BuildingId = buildingId.Value,
            Direction = Direction.Stationary
        };

        await _elevatorRepository.AddAsync(elevator);
        Console.WriteLine($"Elevator '{name}' created successfully! Press any key to return.");
        Console.ReadKey();
    }

    public async Task ViewElevators()
    {
        // CRUD operation: View Elevators
        var elevators = await _elevatorRepository.GetAllAsync();
        Console.WriteLine("\n=== Elevators ===");
        foreach (var elevator in elevators)
        {
            Console.WriteLine($"Name: {elevator.Name}, Max Capacity: {elevator.MaxCapacity}, Building ID: {elevator.BuildingId}");
        }
    }

    public async Task UpdateElevator()
    {
        // Get validated elevator name to update
        var name = await GetElevatorNameAsync();
        if (name == null) return;

        var elevators = await _elevatorRepository.FindAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());

        if (elevators == null || elevators.Count <= 0)
        {
            Console.WriteLine($"Elevator with name: {name} could not be found. Press any key to try again.");
            Console.ReadKey();
            return;
        }

        var elevator = elevators.First();

        // Get new validated elevator details
        name = await GetElevatorNameAsync();
        if (name == null) return;

        var maxCapacity = await GetMaxCapacityAsync();
        if (maxCapacity == null) return;

        var buildingId = await GetBuildingIdAsync();
        if (buildingId == null) return;

        elevator.Name = name;
        elevator.MaxCapacity = maxCapacity.Value;
        elevator.BuildingId = buildingId.Value;

        await _elevatorRepository.UpdateAsync(elevator);
        Console.WriteLine("\nElevator updated successfully! Press any key to return.");
        Console.ReadKey();
    }

    public async Task DeleteElevator()
    {
        // Get validated elevator name to delete
        var name = await GetElevatorNameAsync();
        if (name == null) return;

        var elevators = await _elevatorRepository.FindAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());

        if (elevators == null || elevators.Count <= 0)
        {
            Console.WriteLine($"Elevator with name: {name} could not be found. Press any key to try again.");
            Console.ReadKey();
            return;
        }

        var elevator = elevators.First();
        await _elevatorRepository.RemoveAsync(elevator);

        Console.WriteLine("\nElevator deleted successfully! Press any key to return.");
        Console.ReadKey();
    }

    #region Helper Methods

    // Helper method to prompt for and validate elevator name
    private async Task<string?> GetElevatorNameAsync()
    {
        Console.Write("\nEnter elevator name: ");
        var name = Console.ReadLine();

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Invalid elevator name. Press any key to try again.");
            Console.ReadKey();
            return null;
        }

        await Task.CompletedTask;

        return name;
    }

    // Helper method to prompt for and validate max capacity
    private async Task<int?> GetMaxCapacityAsync()
    {
        Console.Write("Enter elevator max capacity: ");

        if (!int.TryParse(Console.ReadLine(), out var maxCapacity) || maxCapacity <= 0)
        {
            Console.WriteLine("Invalid capacity. Press any key to try again.");
            Console.ReadKey();
            return null;
        }

        await Task.CompletedTask;

        return maxCapacity;
    }

    // Helper method to prompt for and validate building ID
    private async Task<int?> GetBuildingIdAsync()
    {
        Console.Write("Enter building ID: ");

        if (!int.TryParse(Console.ReadLine(), out var buildingId) || buildingId <= 0)
        {
            Console.WriteLine("Invalid building ID. Press any key to try again.");
            Console.ReadKey();
            return null;
        }

        await Task.CompletedTask;

        return buildingId;
    }

    #endregion
}
