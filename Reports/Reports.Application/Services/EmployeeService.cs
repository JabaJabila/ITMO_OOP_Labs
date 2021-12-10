using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.ServicesAbstractions;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        public async Task<Employee> Create(string name)
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

        public async Task<Employee> FindById(Guid id)
        {
            throw new NotImplementedException();
            // return await _context.Employees.FindAsync(id);
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