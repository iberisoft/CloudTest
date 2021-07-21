using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace DeviceConfig.Data
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(DeviceConfig) + ".db");
            optionsBuilder.UseLazyLoadingProxies().UseSqlite($"Data Source={filePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .HasIndex(entity => entity.Name)
                .IsUnique();
            modelBuilder.Entity<Company>()
                .HasIndex(entity => entity.Code)
                .IsUnique();
        }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Network> Networks { get; set; }
    }
}
