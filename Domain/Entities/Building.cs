namespace Domain.Entities;

public class Building
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalFloors { get; set; }

    // Relationships
    public ICollection<Floor> Floors { get; set; } = new List<Floor>();
    public ICollection<Elevator> Elevators { get; set; } = new List<Elevator>();
}
