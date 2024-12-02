using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _buildingRepository;

    public BuildingService(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    public async Task CreateBuildingAsync()
    {
        // Get validated building name and floor count from user
        var name = await GetBuildingNameAsync();
        if (name == null) 
            return;

        var totalFloors = await GetTotalFloorsAsync();
        if (totalFloors == null) 
            return;

        var building = new Building
        {
            Name = name,
            TotalFloors = totalFloors.Value
        };

        await _buildingRepository.AddAsync(building);
        Console.WriteLine($"Building '{name}' created successfully! Press any key to return.");
        Console.ReadKey();
    }

    public async Task ViewBuildings()
    {
        // CRUD operation: View Buildings
        var buildings = await _buildingRepository.GetAllAsync();
        Console.WriteLine("\n=== Buildings ===");
        foreach (var building in buildings)
        {
            Console.WriteLine($"Name: {building.Name}, Total Floors: {building.Floors.Count}, Total Elevators: {building.Elevators.Count}");
        }
    }

    public async Task UpdateBuilding()
    {
        // Get validated building name to update
        var name = await GetBuildingNameAsync();
        if (name == null) return;

        var buildings = await _buildingRepository.FindAsync(b => b.Name.ToLower().Trim() == name.ToLower().Trim());

        if (buildings == null || buildings.Count <= 0)
        {
            Console.WriteLine($"Building with name: {name} could not be found. Press any key to try again.");
            Console.ReadKey();
            return;
        }

        var building = buildings.First();

        // Get new validated building name and floor count
        name = await GetBuildingNameAsync();
        if (name == null) return;

        var totalFloors = await GetTotalFloorsAsync();
        if (totalFloors == null) return;

        building.Name = name;
        building.TotalFloors = totalFloors.Value;

        await _buildingRepository.UpdateAsync(building);
        Console.WriteLine("\nBuilding updated successfully! Press any key to return.");
        Console.ReadKey();
    }

    public async Task DeleteBuilding()
    {
        // Get validated building name to delete
        var name = await GetBuildingNameAsync();
        if (name == null) return;

        var buildings = await _buildingRepository.FindAsync(b => b.Name.ToLower().Trim() == name.ToLower().Trim());

        if (buildings == null || buildings.Count <= 0)
        {
            Console.WriteLine($"Building with name: {name} could not be found. Press any key to try again.");
            Console.ReadKey();
            return;
        }

        var building = buildings.First();
        await _buildingRepository.RemoveAsync(building);

        Console.WriteLine("\nBuilding deleted successfully! Press any key to return.");
        Console.ReadKey();
    }

    #region Helper Methods

    // Helper method to prompt for and validate building name
    private async Task<string?> GetBuildingNameAsync()
    {
        Console.Write("\nEnter building name: ");
        var name = Console.ReadLine();

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Invalid building name. Press any key to try again.");
            Console.ReadKey();
            return null;
        }

        await Task.CompletedTask;

        return name;
    }

    // Helper method to prompt for and validate total floors
    private async Task<int?> GetTotalFloorsAsync()
    {
        Console.Write("Enter building total floors: ");

        if (!int.TryParse(Console.ReadLine(), out var totalFloors) || totalFloors <= 0)
        {
            Console.WriteLine("Invalid floor count. Press any key to try again.");
            Console.ReadKey();
            return null;
        }

        await Task.CompletedTask;

        return totalFloors;
    }

    #endregion
}
