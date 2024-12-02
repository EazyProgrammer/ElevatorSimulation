using Application.Interfaces;

public class CallElevatorScreen : ICallElevatorScreen
{
    private readonly IElevatorService _elevatorService;

    public CallElevatorScreen(IElevatorService elevatorService)
    {
        _elevatorService = elevatorService;
    }

    public async Task Display()
    {
        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.WriteLine();

            // Display elevator statuses
            var statuses = await _elevatorService.GetElevatorStatusesAsync();
            foreach (var status in statuses)
            {
                Console.WriteLine($"{status.ElevatorName} on Floor: {status.CurrentFloor}, Direction: {status.Direction}, Load: {status.CurrentLoad}, Max Capacity: {status.MaxCapacity}");
            }

            Console.WriteLine();
            Console.WriteLine("=== Call Elevator ===");
            Console.WriteLine("[F1] Back to Main Menu");
            Console.WriteLine();

            // Prompt for floor
            Console.Write("\nEnter floor: ");

            // Check for F1 key to go back
            key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.F1)
                return;

            if (!int.TryParse(Console.ReadLine(), out var floor))
            {
                Console.WriteLine("Invalid floor. Press any key to try again.");
                Console.ReadKey();
                continue;
            }

            // Check for F1 key to go back
            key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.F1)
                return;

            // Prompt for number of passengers
            Console.Write("Enter number of passengers: ");
            if (!int.TryParse(Console.ReadLine(), out var passengers))
            {
                Console.WriteLine("Invalid number of passengers. Press any key to try again.");
                Console.ReadKey();
                continue;
            }

            // Call the elevator
            await _elevatorService.CallElevatorAsync(floor, passengers);
            Console.WriteLine("\nElevator called successfully! Press any key to continue.");
            Console.ReadKey();

        } while (key != ConsoleKey.F1);
    }
}
