using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContext
{
    public sealed class ReportsDatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ReportsDatabaseContext(DbContextOptions<ReportsDatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Task>().HasOne(model => model.AssignedEmployee);
            base.OnModelCreating(modelBuilder);
        }
    }
}