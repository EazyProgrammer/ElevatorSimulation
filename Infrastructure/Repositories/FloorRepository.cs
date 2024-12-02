using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FloorRepository : GenericRepository<Floor>, IFloorRepository
    {
        public FloorRepository(AppDbContext context) : base(context) { }

        public Floor GetFloorWithBuilding(int floorId)
        {
            return _context.Set<Floor>()
                .Include(f => f.Building)
                .FirstOrDefault(f => f.Id == floorId) ?? new();
        }

        public IEnumerable<Floor> GetFloorsByBuildingId(int buildingId)
        {
            return _context.Set<Floor>()
                .Where(f => f.BuildingId == buildingId)
                .ToList();
        }

        public IEnumerable<Floor> GetFloorsWithWaitingPassengers(int buildingId)
        {
            return _context.Set<Floor>()
                .Where(f => f.BuildingId == buildingId && f.WaitingPassengers > 0)
                .ToList();
        }
    }

}
