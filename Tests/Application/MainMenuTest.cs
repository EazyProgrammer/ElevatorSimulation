using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Application.Interfaces;
using Xunit;
using Domain.ValueObjects;

namespace Tests.Application;

public class MainMenuTests
{
    private readonly Mock<IElevatorService> _mockElevatorService;
    private readonly Mock<IBuildingService> _mockBuildingService;
    private readonly Mock<IFloorService> _mockFloorService;
    private readonly StringWriter _consoleOutput;
    private readonly MainMenu _mainMenu;

    public MainMenuTests()
    {
        _mockElevatorService = new Mock<IElevatorService>();
        _mockBuildingService = new Mock<IBuildingService>();
        _mockFloorService = new Mock<IFloorService>();
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);  // Redirect console output to StringWriter

        _mainMenu = new MainMenu(
            _mockElevatorService.Object,
            _mockBuildingService.Object,
            _mockFloorService.Object,
            skipClear: true,
            singleRun: true,  // We use a single run for testing
            testMode: true     // Exit early in case of errors
        );
    }

    [Fact]
    public async Task Run_DisplayMainMenu_Test()
    {
        // Arrange
        var userInput = "3"; // Simulate user selecting 'Exit' which triggers the cancellation

        SetConsoleInput(userInput);  // Simulate user input for Exit

        // Act: Run the MainMenu asynchronously
        var task = _mainMenu.Run();

        // Simulate a short delay to let the method run and reach the console output
        await Task.Delay(100);

        // Assert: Check that the main menu is displayed
        Assert.Contains("=== Main Menu ===", _consoleOutput.ToString());
        Assert.Contains("[1] Call Elevator", _consoleOutput.ToString());
        Assert.Contains("[2] Admin Menu", _consoleOutput.ToString());
        Assert.Contains("[3] Exit", _consoleOutput.ToString());

        // Assert: Ensure that the task completes successfully by catching the OperationCanceledException
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => task);
        Assert.Equal("Exit selected", exception.Message);
    }

    [Fact]
    public async Task CallElevatorOption_Successful_Call_Test()
    {
        // Arrange
        var userInput = "1\n2\n2\n"; // Simulate user selecting 'Call Elevator' and entering floor and passengers

        // Set up Console input to simulate key presses in sequence
        SetConsoleInput(userInput);

        // Mock the elevator service's CallElevatorAsync method to complete successfully
        _mockElevatorService.Setup(s => s.CallElevatorAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);  // Simulate a successful call

        // Mock the elevator service's GetElevatorStatusesAsync method (ensure this is not blocking)
        _mockElevatorService.Setup(s => s.GetElevatorStatusesAsync())
            .ReturnsAsync(new List<ElevatorStatus> { new ElevatorStatus { ElevatorName = "Elevator1", CurrentFloor = 1, Direction = Domain.Enums.Direction.Up, CurrentLoad = 3, MaxCapacity = 10 } });

        // Act: Run the MainMenu asynchronously
        var task = _mainMenu.Run();

        // Simulate a short delay to allow the async process to execute
        await Task.Delay(100);

        // Assert: Verify that CallElevatorAsync was called exactly once
        _mockElevatorService.Verify(m => m.CallElevatorAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

        // Assert: Ensure the output contains "Elevator called successfully!" message
        Assert.Contains("Elevator called successfully!", _consoleOutput.ToString());

        // Ensure the task completes successfully after the method executes
        await task;
    }

    [Fact]
    public async Task CallElevatorOption_InvalidInput_Test()
    {
        // Arrange
        var userInput = "1\ninvalid\n2\n"; // Invalid floor input
        SetConsoleInput(userInput);  // Simulate user input for Call Elevator

        // Act
        await _mainMenu.Run();

        // Assert
        Assert.Contains("Invalid floor. Press any key to try again.", _consoleOutput.ToString());
    }

    [Fact]
    public async Task AdminMenuOption_Test()
    {
        // Arrange
        var userInput = "2\n4\n";  // Choose Admin Menu option and Return to Main Menu

        // Set up Console input to simulate key presses in sequence
        SetConsoleInput(userInput);

        // Mocking the relevant service calls if necessary
        _mockElevatorService.Setup(s => s.GetElevatorStatusesAsync())
            .ReturnsAsync(new List<ElevatorStatus> { new ElevatorStatus { ElevatorName = "Elevator1", CurrentFloor = 1, Direction = Domain.Enums.Direction.Up, CurrentLoad = 3, MaxCapacity = 10 } });

        // Act: Run the MainMenu asynchronously
        var task = _mainMenu.Run();

        // Simulate a short delay to allow the async process to execute
        await Task.Delay(100);

        // Assert: Check if the "Admin Menu" is printed in the output
        Assert.Contains("=== Admin Menu ===", _consoleOutput.ToString());

        // Assert: Ensure that the flow eventually returns to the Main Menu after selecting "Return to Main Menu"
        Assert.Contains("=== Main Menu ===", _consoleOutput.ToString());

        // Ensure the task completes successfully after the method executes
        await task;
    }

    [Fact]
    public async Task ExitApplicationOption_Test()
    {
        // Arrange
        var userInput = "3\n";  // Choose Exit option
        SetConsoleInput(userInput);  // Simulate user input for Exit

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => _mainMenu.Run());
        Assert.Equal("Exit selected", exception.Message);
    }

    private void SetConsoleInput(string input)
    {
        var stringReader = new StringReader(input);
        Console.SetIn(stringReader);
    }
}

public class ConsoleInputHelper
{
    private readonly Queue<ConsoleKeyInfo> _inputs = new Queue<ConsoleKeyInfo>();

    public ConsoleInputHelper(params ConsoleKeyInfo[] inputs)
    {
        foreach (var input in inputs)
        {
            _inputs.Enqueue(input);
        }
    }

    public ConsoleKeyInfo ReadKey()
    {
        if (_inputs.Count == 0)
            throw new InvalidOperationException("No more input available.");

        return _inputs.Dequeue();
    }
}
