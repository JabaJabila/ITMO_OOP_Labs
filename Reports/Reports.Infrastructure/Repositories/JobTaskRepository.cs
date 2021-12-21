using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
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
                .ThenInclude(c => c.Author)
                .Include(t => t.AssignedEmployee)
                .ToListAsync();
        }

        public async Task<JobTask> GetById(Guid id)
        {
            return (await GetAll()).FirstOrDefault(t => t.Id == id);
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