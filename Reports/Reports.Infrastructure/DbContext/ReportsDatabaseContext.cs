using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContext
{
    public sealed class ReportsDatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ReportsDatabaseContext(DbContextOptions<ReportsDatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<TaskModel>().HasOne(model => model.AssignedEmployee);
            base.OnModelCreating(modelBuilder);
        }
    }
}