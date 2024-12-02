using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BuildingRepository : GenericRepository<Building>, IBuildingRepository
    {
        public BuildingRepository(AppDbContext context) : base(context) { }

        public Building GetBuildingWithFloorsAndElevators(int id)
        {
            return _context.Set<Building>()
                .Include(b => b.Floors)
                .Include(b => b.Elevators)
                .FirstOrDefault(b => b.Id == id) ?? new();
        }
    }
}
