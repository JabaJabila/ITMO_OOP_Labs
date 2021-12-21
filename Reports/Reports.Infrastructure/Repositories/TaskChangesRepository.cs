using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities.TaskChanges;
using Core.RepositoryAbstractions;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories
{
    public sealed class TaskChangesRepository : ITaskChangesRepository
    {
        private readonly ReportsDatabaseContext _context;

        public TaskChangesRepository(ReportsDatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async void Dispose()
        {
            await _context.DisposeAsync();
        }

        public async Task<IReadOnlyCollection<JobTaskChange>> GetAll()
        {
            return await _context.TaskChanges
                .Include(c => c.Author)
                .ToListAsync();
        }

        public async Task<JobTaskChange> GetById(Guid id)
        {
            return await _context.TaskChanges.FindAsync(id);
        }

        public async Task<JobTaskChange> Add(JobTaskChange entity)
        {
            EntityEntry<JobTaskChange> change = await _context.TaskChanges.AddAsync(entity);
            await SaveChanges();
            return change.Entity;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}