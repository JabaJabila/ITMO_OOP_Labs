using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.ServicesAbstractions;

namespace Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        public async Task<Employee> CreateEmployee(string name)
        {
            throw new NotImplementedException();
            // var employee = new Employee(Guid.NewGuid(), name);
            // await _context.Employees.AddAsync(employee);
            // await _context.SaveChangesAsync();
            // return employee;
        }

        public async Task<Employee> FindByName(string name)
        {
            throw new NotImplementedException();
            // return await _context.Employees.FirstOrDefaultAsync(
            //     employee => employee.Name == name);
        }

        public async Task<Employee> GetById(Guid id)
        {
            throw new NotImplementedException();
            // return await _context.Employees.FindAsync(id);
        }

        public Task<List<Employee>> GetAll()
        {
            throw new NotImplementedException();
        }

        Task IEmployeeService.Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task AddSubordinate(Employee currentEmployee, Employee subordinate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSubordinate(Employee currentEmployee, Employee subordinate)
        {
            throw new NotImplementedException();
        }

        public Task SetSupervisor(Employee currentEmployee, Employee supervisor)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Employee Update(Employee entity)
        {
            throw new NotImplementedException();
        }
    }
}