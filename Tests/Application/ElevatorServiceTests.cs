using Application.Services;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;
using Moq;

namespace Tests.Application;

public class ElevatorServiceTests
{
    private readonly Mock<IElevatorRepository> _mockElevatorRepository;
    private readonly Mock<IFloorRepository> _mockFloorRepository;
    private readonly ElevatorService _elevatorService;

    public ElevatorServiceTests()
    {
        _mockElevatorRepository = new Mock<IElevatorRepository>();
        _mockFloorRepository = new Mock<IFloorRepository>();
        _elevatorService = new ElevatorService(_mockElevatorRepository.Object, _mockFloorRepository.Object);
    }

    [Fact]
    public async Task GetElevatorStatusesAsync_ShouldReturnElevatorStatuses()
    {
        // Arrange
        var elevators = new List<Elevator>
        {
            new Elevator { Name = DefaultElevatorName.ElevatorA, CurrentFloor = 1, Direction = Direction.Stationary, CurrentLoad = 0, MaxCapacity = 10 },
            new Elevator { Name = DefaultElevatorName.ElevatorB, CurrentFloor = 5, Direction = Direction.Up, CurrentLoad = 3, MaxCapacity = 10 }
        };
        _mockElevatorRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(elevators);

        // Act
        var statuses = await _elevatorService.GetElevatorStatusesAsync();

        // Assert
        Assert.NotNull(statuses);
        Assert.Equal(2, statuses.Count());
        Assert.Contains(statuses, s => s.ElevatorName == DefaultElevatorName.ElevatorA && s.CurrentFloor == 1);
        Assert.Contains(statuses, s => s.ElevatorName == DefaultElevatorName.ElevatorB && s.CurrentFloor == 5);
    }

    [Fact]
    public async Task SimulateMovementAsync_ShouldMoveElevatorToTargetFloor()
    {
        // Arrange
        var elevatorRepositoryMock = new Mock<IElevatorRepository>();
        var floorRepositoryMock = new Mock<IFloorRepository>();

        // Mock the elevator data
        var elevator = new Elevator
        {
            Name = DefaultElevatorName.ElevatorA,
            CurrentFloor = 1,
            Direction = Direction.Stationary,
            IsInMotion = false,
            CurrentLoad = 0,
            MaxCapacity = 10,
            TargetFloors = new List<int> { 3 }
        };

        // Mock the repository behavior
        elevatorRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Elevator>())).Returns(Task.CompletedTask);
        elevatorRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Elevator> { elevator });

        var elevatorService = new ElevatorService(elevatorRepositoryMock.Object, floorRepositoryMock.Object);

        // Capture the Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await elevatorService.SimulateMovementAsync(elevator);

        // Allow some time for movement and logging
        await Task.Delay(2000); // Adjust this delay as necessary to wait for elevator movement to complete

        // Assert
        var consoleOutput = stringWriter.ToString();

        // Check if the elevator arrived at floor 3
        Assert.Contains($"{elevator.Name} arrived at floor 3.", consoleOutput);

        // Check if the elevator completed the request
        Assert.Contains($"{elevator.Name} has completed all requests.", consoleOutput);

        // Optionally check for movement logs
        Assert.Contains($"{DefaultElevatorName.ElevatorA} at floor 2, Status: Up", consoleOutput);
        Assert.Contains($"{DefaultElevatorName.ElevatorA} at floor 3, Status: Stationary", consoleOutput);

        // Ensure the elevator's state was updated during the movement simulation
        elevatorRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Elevator>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task CallElevatorAsync_ShouldAssignNearestAvailableElevator()
    {
        // Arrange
        var elevatorA = new Elevator
        {
            Name = DefaultElevatorName.ElevatorA,
            CurrentFloor = 2,
            Direction = Direction.Stationary,
            CurrentLoad = 0,
            MaxCapacity = 10,
            TargetFloors = new List<int>()
        };

        var elevatorB = new Elevator
        {
            Name = DefaultElevatorName.ElevatorB,
            CurrentFloor = 6,
            Direction = Direction.Stationary,
            CurrentLoad = 3,
            MaxCapacity = 10,
            TargetFloors = new List<int>()
        };

        var elevators = new List<Elevator> { elevatorA, elevatorB };

        _mockElevatorRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(elevators);
        _mockElevatorRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Elevator>())).Callback<Elevator>(elevator =>
        {
            // Ensure that the target floor is added for the selected elevator
            if (elevator.Name == DefaultElevatorName.ElevatorA && elevator.TargetFloors.Any())
            {
                Assert.Contains(4, elevator.TargetFloors); // Target floor 4 must be in the list
            }
        });

        // Act
        await _elevatorService.CallElevatorAsync(4, 2);

        // Assert
        _mockElevatorRepository.Verify(repo => repo.UpdateAsync(It.Is<Elevator>(e => e.Name == DefaultElevatorName.ElevatorA)), Times.AtLeastOnce);
    }

    [Fact]
    public async Task CallElevatorAsync_ShouldNotAssignWhenNoElevatorAvailable()
    {
        // Arrange
        var mockElevatorRepository = new Mock<IElevatorRepository>();
        mockElevatorRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Elevator>()); // No elevators available

        var mockFloorRepository = new Mock<IFloorRepository>();

        var service = new ElevatorService(mockElevatorRepository.Object, mockFloorRepository.Object);

        using var consoleOutput = new StringWriter();
        var originalOutput = Console.Out;
        Console.SetOut(consoleOutput);

        try
        {
            // Act
            await service.CallElevatorAsync(5, 3); // Request elevator to floor 5 with 3 passengers

            // Assert
            var output = consoleOutput.ToString();
            Assert.Contains("No available elevator. Please wait.", output);
        }
        finally
        {
            // Cleanup
            Console.SetOut(originalOutput);
        }
    }


    [Fact]
    public async Task SimulateMovementAsync_ShouldLogMovement()
    {
        // Arrange
        var elevator = new Elevator
        {
            Name = DefaultElevatorName.ElevatorA,
            CurrentFloor = 1,
            Direction = Direction.Stationary,
            CurrentLoad = 2,
            MaxCapacity = 10,
            TargetFloors = new List<int> { 3 }
        };

        _mockElevatorRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Elevator>())).Returns(Task.CompletedTask);

        using var output = new StringWriter();
        Console.SetOut(output);

        // Act
        await _elevatorService.SimulateMovementAsync(elevator);

        // Assert
        var logs = output.ToString();
        Assert.Contains($"{DefaultElevatorName.ElevatorA} at floor 2, Status: Up", logs);
        Assert.Contains($"{DefaultElevatorName.ElevatorA} has completed all requests", logs);
    }
}
