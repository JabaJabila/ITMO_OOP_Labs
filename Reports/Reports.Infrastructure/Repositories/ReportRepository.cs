using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.RepositoryAbstractions;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories
{
    public sealed class ReportRepository : IReportRepository
    {
        private readonly ReportsDatabaseContext _context;

        public ReportRepository(ReportsDatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async void Dispose()
        {
            await _context.DisposeAsync();
        }

        public async Task<IReadOnlyCollection<Report>> GetAll()
        {
            return await _context.Reports
                .Include(r => r.AssignedEmployee)
                .Include(r => r.Tasks)
                .ToListAsync();
        }

        public async Task<Report> GetById(Guid id)
        {
            Report report = await _context.Reports.FindAsync(id);
            await _context.Entry(report).Reference(r => r.AssignedEmployee).LoadAsync();
            await _context.Entry(report).Collection(r => r.Tasks).LoadAsync();
            return report;
        }

        public async Task<Report> Add(Report entity)
        {
            EntityEntry<Report> report = await _context.AddAsync(entity);
            await SaveChanges();
            return report.Entity;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}