using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IBuildingRepository : IGenericRepository<Building>
{
    Building GetBuildingWithFloorsAndElevators(int id);
}
