using Domain.Enums;

namespace Domain.ValueObjects;

public class ElevatorStatus
{
    public string ElevatorName { get; set; } = string.Empty;
    public int CurrentFloor { get; set; }
    public Direction Direction { get; set; }
    public bool IsInMotion { get; set; }
    public int CurrentLoad { get; set; }
    public int MaxCapacity { get; set; }
}
