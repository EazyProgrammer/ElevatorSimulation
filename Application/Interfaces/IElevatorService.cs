using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IElevatorService
    {
        Task<IEnumerable<ElevatorStatus>> GetElevatorStatusesAsync();
        Task CallElevatorAsync(int floor, int passengers);
        Task CreateElevatorAsync();
        Task ViewElevators();
        Task UpdateElevator();
        Task DeleteElevator();
    }
}
