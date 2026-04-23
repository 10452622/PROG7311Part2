using Microsoft.EntityFrameworkCore;
using PROG7311Part2.Models;

namespace PROG7311Part2.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.Cost)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
