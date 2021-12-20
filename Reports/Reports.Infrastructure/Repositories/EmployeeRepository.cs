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
    public sealed class EmployeeRepository : IEmployeeRepository
    {
        private readonly ReportsDatabaseContext _context;

        public EmployeeRepository(ReportsDatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async void Dispose()
        {
            await _context.DisposeAsync();
        }

        public async Task<IReadOnlyCollection<Employee>> GetAll()
        {
            return await _context.Employees.Where(e => e.Status != EmployeeStatus.Fired).ToListAsync();
        }

        public async Task<Employee> GetById(Guid id)
        {
            Employee employee = await _context.Employees.FindAsync(id);
            return employee.Status == EmployeeStatus.Fired ? null : employee;
        }

        public async Task<Employee> Add(Employee entity)
        {
            EntityEntry<Employee> employee = await _context.AddAsync(entity);
            await SaveChanges();
            return employee.Entity;
        }

        public async Task Delete(Guid id)
        {
            Employee employeeToDelete = await GetById(id);
            employeeToDelete.Status = EmployeeStatus.Fired;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Employee> FindByName(string name)
        {
            return await _context.Employees.FirstOrDefaultAsync(
                e => e.Name == name && e.Status != EmployeeStatus.Fired);
        }
    }
}