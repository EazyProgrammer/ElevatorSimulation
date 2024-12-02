using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IFloorRepository : IGenericRepository<Floor>
{
    Floor GetFloorWithBuilding(int floorId);
    IEnumerable<Floor> GetFloorsByBuildingId(int buildingId);
    IEnumerable<Floor> GetFloorsWithWaitingPassengers(int buildingId);
}
