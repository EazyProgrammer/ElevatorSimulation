using Application.Interfaces;

public class MainMenu : IMainMenu
{
    private readonly IElevatorService _elevatorService;
    private readonly IBuildingService _buildingService;
    private readonly IFloorService _floorService;
    private readonly bool _skipClear;
    private readonly bool _singleRun;
    private readonly bool _testMode;

    public MainMenu(IElevatorService elevatorService, IBuildingService buildingService, IFloorService floorService,  bool skipClear = false, bool singleRun = false, bool testMode = false)
    {
        _elevatorService = elevatorService;
        _buildingService = buildingService;
        _floorService = floorService;
        _skipClear = skipClear;
        _singleRun = singleRun;
        _testMode = testMode;
    }

    public async Task Run()
    {
        do
        {
            if (!_skipClear)
                Console.Clear();

            // Display Menu Options and Elevator Statuses
            await DisplayMenuAndStatuses();

            // Display Main Menu with Admin Option
            DisplayMainMenu();

            // Read user input for main menu option
            var input = Console.ReadLine();

            try
            {
                if (input == "1") // Option 1: Call Elevator
                {
                    await CallElevatorOption();
                }
                else if (input == "2") // Option 2: Admin Menu
                {
                    await AdminMenuOption();
                }
                else if (input == "3") // Option 3: Exit
                {
                    ExitApplicationOption();
                }
                else
                {
                    DefaultOption();
                }
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException) throw;
                Console.WriteLine($"Error: {ex.Message}. Press any key to continue...");
                if (_testMode) 
                    return; // Exit for tests
                Console.ReadKey();
            }
        } while (!_singleRun);
    }

    private static void DisplayMainMenu()
    {
        // Display Main Menu
        Console.WriteLine("\n=== Main Menu ===");
        Console.WriteLine("[1] Call Elevator");
        Console.WriteLine("[2] Admin Menu");
        Console.WriteLine("[3] Exit\n");
        Console.Write("Enter choice: ");
    }

    private async Task CallElevatorOption()
    {
        // Call Elevator: Prompt for floor and passengers
        Console.Write("\nEnter floor: ");
        if (!int.TryParse(Console.ReadLine(), out var floor))
        {
            Console.WriteLine("Invalid floor. Press any key to try again.");
            if (_testMode)
                return; // Exit for tests
            Console.ReadKey();
            return;
        }

        Console.Write("Enter number of passengers: ");
        if (!int.TryParse(Console.ReadLine(), out var passengers))
        {
            Console.WriteLine("Invalid number of passengers. Press any key to try again.");
            if (_testMode)
                return; // Exit for tests
            Console.ReadKey();
            return;
        }

        // Call the elevator
        await _elevatorService.CallElevatorAsync(floor, passengers);
        Console.WriteLine("\nElevator called successfully! Press any key to return.");

        if (_testMode)
            return; // Exit for tests

        Console.ReadKey();
    }

    private async Task AdminMenuOption()
    {
        // Display Admin Menu for managing buildings, elevators, and floors

        if (!_skipClear)
            Console.Clear();

        Console.WriteLine("=== Admin Menu ===");
        Console.WriteLine("[1] Manage Buildings");
        Console.WriteLine("[2] Manage Elevators");
        Console.WriteLine("[3] Manage Floors");
        Console.WriteLine("[4] Return to Main Menu");

        var choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    await ManageBuildings();
                    break;
                case "2":
                    await ManageElevators();
                    break;
                case "3":
                    await ManageFloors();
                    break;
                case "4":
                    return; // Return to Main Menu
                default:
                    DefaultOption();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}. Press any key to continue...");
            if (_testMode) 
                return; // Exit for tests
            Console.ReadKey();
        }
    }

    private async Task ManageBuildings()
    {
        // Manage Buildings - CRUD operations
        Console.Clear();

        await _buildingService.ViewBuildings();

        Console.WriteLine();
        Console.WriteLine("=== Manage Buildings ===");
        Console.WriteLine("[1] Create Building");
        Console.WriteLine("[2] Update Building");
        Console.WriteLine("[3] Delete Building");
        Console.WriteLine("[4] Return to Admin Menu");
        Console.WriteLine();

        var choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    await _buildingService.CreateBuildingAsync();
                    break;
                case "2":
                    await _buildingService.UpdateBuilding();
                    break;
                case "3":
                    await _buildingService.DeleteBuilding();
                    break;
                case "4":
                    await AdminMenuOption();
                    break; // Return to Admin Menu
                default:
                    DefaultOption();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}. Press any key to continue...");
            if (_testMode) return; // Exit for tests
            Console.ReadKey();
        }
    }

    private async Task ManageElevators()
    {
        // Manage Elevators - CRUD operations
        Console.Clear();

        await _elevatorService.ViewElevators();

        Console.WriteLine();
        Console.WriteLine("=== Manage Elevators ===");
        Console.WriteLine("[1] Create Elevator");
        Console.WriteLine("[3] Update Elevator");
        Console.WriteLine("[4] Delete Elevator");
        Console.WriteLine("[5] Return to Admin Menu");

        var choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    await _elevatorService.CreateElevatorAsync();
                    break;
                case "2":
                    await _elevatorService.UpdateElevator();
                    break;
                case "3":
                    await _elevatorService.DeleteElevator();
                    break;
                case "4":
                    await AdminMenuOption();
                    break; // Return to Admin Menu
                default:
                    DefaultOption();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}. Press any key to continue...");
            if (_testMode) return; // Exit for tests
            Console.ReadKey();
        }
    }

    private async Task ManageFloors()
    {
        // Manage Floors - CRUD operations
        Console.Clear();

        await _floorService.ViewFloors();

        Console.WriteLine();
        Console.WriteLine("=== Manage Floors ===");
        Console.WriteLine("[1] Create Floor");
        Console.WriteLine("[3] Update Floor");
        Console.WriteLine("[4] Delete Floor");
        Console.WriteLine("[5] Return to Admin Menu");

        var choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    await _floorService.CreateFloorAsync();
                    break;
                case "2":
                    await _floorService.UpdateFloor();
                    break;
                case "3":
                    await _floorService.DeleteFloor();
                    break;
                case "4":
                    await AdminMenuOption();
                    break; // Return to Admin Menu
                default:
                    DefaultOption();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}. Press any key to continue...");
            if (_testMode) return; // Exit for tests
            Console.ReadKey();
        }
    }

    private async Task DisplayMenuAndStatuses()
    {
        // Display elevator statuses
        var statuses = await _elevatorService.GetElevatorStatusesAsync();
        Console.WriteLine("=== Elevator Statuses ===");
        foreach (var status in statuses)
        {
            Console.WriteLine($"{status.ElevatorName} on Floor: {status.CurrentFloor}, Direction: {status.Direction}, Load: {status.CurrentLoad}, Max Capacity: {status.MaxCapacity}");
        }
    }

    public void DefaultOption()
    {
        Console.WriteLine("Invalid option. Press any key to continue...");
        if (_testMode) 
            return; // Exit for tests
        Console.ReadKey();
    }

    private static void ExitApplicationOption()
    {
        throw new OperationCanceledException("Exit selected");
    }
}
