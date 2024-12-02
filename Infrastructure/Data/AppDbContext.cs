using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Elevator> Elevators { get; set; }
    public DbSet<Floor> Floors { get; set; }
    public DbSet<Building> Buildings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlite("Data Source=elevator.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Building entity configuration
        modelBuilder.Entity<Building>()
            .HasMany(b => b.Floors)
            .WithOne(f => f.Building)
            .HasForeignKey(f => f.BuildingId);

        modelBuilder.Entity<Building>()
            .HasMany(b => b.Elevators);

        // Floor entity configuration
        modelBuilder.Entity<Floor>()
            .HasOne(f => f.Building)
            .WithMany(b => b.Floors)
            .HasForeignKey(f => f.BuildingId);
    }
}
