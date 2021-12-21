using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Tools;
using Core.RepositoryAbstractions;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories
{
    public sealed class JobTaskRepository : IJobTaskRepository
    {
        private readonly ReportsDatabaseContext _context;

        public JobTaskRepository(ReportsDatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async void Dispose()
        {
            await _context.DisposeAsync();
        }

        public async Task<IReadOnlyCollection<JobTask>> GetAll()
        {
            return await _context.JobTasks
                .Include(t => t.Changes)
                .Include(t => t.AssignedEmployee)
                .ToListAsync();
        }

        public async Task<JobTask> GetById(Guid id)
        {
            JobTask task = await _context.JobTasks.FindAsync(id);
            await _context.Entry(task).Collection(t => t.Changes).LoadAsync();
            await _context.Entry(task).Reference(t => t.AssignedEmployee).LoadAsync();
            return task;
        }

        public async Task<JobTask> Add(JobTask entity)
        {
            EntityEntry<JobTask> task = await _context.JobTasks.AddAsync(entity);
            await SaveChanges();
            return task.Entity;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}