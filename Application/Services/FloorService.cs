using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services;

public class FloorService : IFloorService
{
    private readonly IFloorRepository _floorRepository;

    public FloorService(IFloorRepository floorRepository)
    {
        _floorRepository = floorRepository;
    }

    public async Task CreateFloorAsync()
    {
        // Get validated floor details from user
        var floorNumber = await GetFloorNumberAsync();
        if (floorNumber == null) return;

        var waitingPassengers = await GetWaitingPassengersAsync();
        if (waitingPassengers == null) return;

        var buildingId = await GetBuildingIdAsync();
        if (buildingId == null) return;

        var floor = new Floor
        {
            FloorNumber = floorNumber.Value,
            WaitingPassengers = waitingPassengers.Value,
            BuildingId = buildingId.Value
        };

        await _floorRepository.AddAsync(floor);
        Console.WriteLine($"Floor {floor.FloorNumber} created successfully! Press any key to return.");
        Console.ReadKey();
    }

    public async Task ViewFloors()
    {
        // CRUD operation: View Floors
        var floors = await _floorRepository.GetAllAsync();
        Console.WriteLine("\n=== Floors ===");
        foreach (var floor in floors)
        {
            Console.WriteLine($"Floor: {floor.FloorNumber}, Building ID: {floor.BuildingId}, Waiting Passengers: {floor.WaitingPassengers}");
        }
    }

    public async Task UpdateFloor()
    {
        // Get validated floor number to update
        var floorNumber = await GetFloorNumberAsync();
        if (floorNumber == null) return;

        var floors = await _floorRepository.FindAsync(f => f.FloorNumber == floorNumber.Value);

        if (floors == null || floors.Count <= 0)
        {
            Console.WriteLine($"Floor with number: {floorNumber} could not be found. Press any key to try again.");
            Console.ReadKey();
            return;
        }

        var floor = floors.First();

        // Get new validated floor details
        floorNumber = await GetFloorNumberAsync();
        if (floorNumber == null) return;

        var waitingPassengers = await GetWaitingPassengersAsync();
        if (waitingPassengers == null) return;

        var buildingId = await GetBuildingIdAsync();
        if (buildingId == null) return;

        floor.FloorNumber = floorNumber.Value;
        floor.WaitingPassengers = waitingPassengers.Value;
        floor.BuildingId = buildingId.Value;

        await _floorRepository.UpdateAsync(floor);
        Console.WriteLine("\nFloor updated successfully! Press any key to return.");
        Console.ReadKey();
    }

    public async Task DeleteFloor()
    {
        // Get validated floor number to delete
        var floorNumber = await GetFloorNumberAsync();
        if (floorNumber == null) return;

        var floors = await _floorRepository.FindAsync(f => f.FloorNumber == floorNumber.Value);

        if (floors == null || floors.Count <= 0)
        {
            Console.WriteLine($"Floor with number: {floorNumber} could not be found. Press any key to try again.");
            Console.ReadKey();
            return;
        }

        var floor = floors.First();
        await _floorRepository.RemoveAsync(floor);

        Console.WriteLine("\nFloor deleted successfully! Press any key to return.");
        Console.ReadKey();
    }

    #region Helper Methods

    // Helper method to prompt for and validate floor number
    private async Task<int?> GetFloorNumberAsync()
    {
        Console.Write("\nEnter floor number: ");
        if (!int.TryParse(Console.ReadLine(), out var floorNumber) || floorNumber <= 0)
        {
            Console.WriteLine("Invalid floor number. Press any key to try again.");
            Console.ReadKey();
            return null;
        }
        return floorNumber;
    }

    // Helper method to prompt for and validate waiting passengers
    private async Task<int?> GetWaitingPassengersAsync()
    {
        Console.Write("Enter waiting passengers: ");
        if (!int.TryParse(Console.ReadLine(), out var waitingPassengers) || waitingPassengers < 0)
        {
            Console.WriteLine("Invalid number of waiting passengers. Press any key to try again.");
            Console.ReadKey();
            return null;
        }
        return waitingPassengers;
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
        return buildingId;
    }

    #endregion
}
