using Core.Domain.Entities;
using Core.Domain.Entities.TaskChanges;
using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

            modelBuilder.Entity<Employee>()
                .Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<EmployeeStatus>());
            
            modelBuilder.Entity<JobTask>()
                .Property(t => t.CurrentState)
                .HasConversion(new EnumToStringConverter<JobTaskState>());
            
            modelBuilder.Entity<Report>()
                .Property(r => r.State)
                .HasConversion(new EnumToStringConverter<ReportState>());
            
            modelBuilder.Entity<Report>()
                .Property(r => r.Type)
                .HasConversion(new EnumToStringConverter<ReportType>());
            
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<JobTask>().ToTable("JobTasks");
            modelBuilder.Entity<Report>().ToTable("Reports");
            modelBuilder.Entity<AssignedEmployeeChange>().ToTable("TaskChanges");
            modelBuilder.Entity<CommentChange>().ToTable("TaskChanges");
            modelBuilder.Entity<DescriptionChange>().ToTable("TaskChanges");
            modelBuilder.Entity<StateChange>().ToTable("TaskChanges");
            
            base.OnModelCreating(modelBuilder);
        }
    }
}