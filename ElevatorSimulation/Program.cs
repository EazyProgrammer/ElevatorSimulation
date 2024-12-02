using Application.Interfaces;
using ElevatorSimulation;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var serviceProvider = DependencyInjection.ConfigureServices(services);
DependencyInjection.ConfigureDatabase(serviceProvider);
var mainMenu = serviceProvider.GetRequiredService<IMainMenu>();
await mainMenu.Run();