using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAdminMenu
    {
        Task AddNewElevator();
        Task AddNewFloor();
        Task RunAdminMenu();
        Task ViewElevatorStatuses();
    }
}
