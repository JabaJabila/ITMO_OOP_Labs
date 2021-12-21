using Core.Domain.Entities;
using Core.Domain.Entities.TaskChanges;
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
        public DbSet<JobTask> JobTasks { get; set; }
        public DbSet<JobTaskChange> TaskChanges { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobTask>()
                .HasOne(t => t.AssignedEmployee);

            modelBuilder.Entity<JobTask>()
                .HasMany(t => t.Changes);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.AssignedEmployee);

            modelBuilder.Entity<Report>()
                .HasMany(r => r.Tasks);

            modelBuilder.Entity<JobTaskChange>()
                .HasOne(c => c.Author);
            
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<JobTask>().ToTable("JobTasks");
            modelBuilder.Entity<JobTaskChange>().ToTable("TaskChanges");
            modelBuilder.Entity<Report>().ToTable("Reports");
            
            base.OnModelCreating(modelBuilder);
        }
    }
}