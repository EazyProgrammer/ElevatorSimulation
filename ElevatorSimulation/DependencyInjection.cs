using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorSimulation
{
    public static class DependencyInjection
    {
        public static IServiceProvider ConfigureServices(this ServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=elevator.db"));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IElevatorRepository, ElevatorRepository>();
            services.AddScoped<IDataSeeder, DataSeeder>();
            services.AddScoped<IFloorRepository, FloorRepository>();
            services.AddScoped<IElevatorService, ElevatorService>();
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IFloorService, FloorService>();
            services.AddScoped<IMainMenu, MainMenu>();

            return services.BuildServiceProvider();
        }

        public static void ConfigureDatabase(this IServiceProvider services)
        {
            var dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
                Console.WriteLine($"Created folder: {dataFolder}");
            }

            var context = services.GetRequiredService<AppDbContext>();
            context.Database.Migrate(); // Apply migrations
            Console.WriteLine("Database created and migrations applied.");

            var seeder = services.GetRequiredService<IDataSeeder>();
            seeder.Seed(context);
        }
    }
}
