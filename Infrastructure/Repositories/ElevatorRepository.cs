using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ElevatorRepository : GenericRepository<Elevator>, IElevatorRepository
    {
        public ElevatorRepository(AppDbContext context) : base(context) { }

        public Elevator GetElevatorWithBuilding(int elevatorId)
        {
            return _context.Set<Elevator>()
                .Include(e => e.Building)
                .FirstOrDefault(e => e.Id == elevatorId) ?? new();
        }

        public IEnumerable<Elevator> GetElevatorsByBuildingId(int buildingId)
        {
            return _context.Set<Elevator>()
                .Where(e => e.BuildingId == buildingId)
                .ToList();
        }

        public IEnumerable<Elevator> GetIdleElevators(int buildingId)
        {
            return _context.Set<Elevator>()
                .Where(e => e.BuildingId == buildingId && !e.IsInMotion)
                .ToList();
        }
    }
}
