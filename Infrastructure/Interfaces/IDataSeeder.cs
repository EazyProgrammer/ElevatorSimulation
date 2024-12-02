using Infrastructure.Data;

namespace Infrastructure.Interfaces;

public interface IDataSeeder
{
    Task Seed(AppDbContext context);
}
