using Microsoft.EntityFrameworkCore;
using TrafficRegister.Models;

namespace TrafficRegister.Data;

public sealed class TrafficDbContext(DbContextOptions<TrafficDbContext> options) : DbContext(options)
{
    public DbSet<Inspector> Inspectors => Set<Inspector>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<OffenseArticle> Articles => Set<OffenseArticle>();
    public DbSet<Violation> Violations => Set<Violation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inspector>().HasIndex(x => x.BadgeNumber).IsUnique();
        modelBuilder.Entity<Vehicle>().HasIndex(x => x.PlateNumber).IsUnique();
        modelBuilder.Entity<Violation>().HasIndex(x => x.CaseNumber).IsUnique();

        modelBuilder.Entity<Violation>()
            .HasMany(x => x.Inspectors).WithMany(x => x.Violations)
            .UsingEntity(join => join.ToTable("ViolationInspector"));

        modelBuilder.Entity<Violation>()
            .HasMany(x => x.Articles).WithMany(x => x.Violations)
            .UsingEntity(join => join.ToTable("ViolationArticle"));
    }
}
