using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IElevatorRepository : IGenericRepository<Elevator>
{
    Elevator GetElevatorWithBuilding(int elevatorId);
    IEnumerable<Elevator> GetElevatorsByBuildingId(int buildingId);
    IEnumerable<Elevator> GetIdleElevators(int buildingId);
}
