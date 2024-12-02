namespace Domain.Entities;

public class Floor
{
    public int Id { get; set; }
    public int FloorNumber { get; set; }
    public int WaitingPassengers { get; set; } = 0;

    // Relationships
    public int BuildingId { get; set; }
    public Building Building { get; set; } = new();
}
