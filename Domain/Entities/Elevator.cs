using Domain.Enums;

namespace Domain.Entities;

public class Elevator
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CurrentFloor { get; set; }
    public bool IsInMotion { get; set; } = false;
    public int MaxCapacity { get; set; }
    public int CurrentLoad { get; set; } = 0;
    public Direction Direction { get; set; }
    public List<int> TargetFloors { get; set; } = new List<int>();

    public int BuildingId { get; set; }
    public Building Building { get; set; } = new();
}
