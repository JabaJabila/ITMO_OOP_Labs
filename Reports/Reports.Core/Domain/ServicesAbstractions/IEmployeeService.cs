using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Domain.ServicesAbstractions
{
    public interface IEmployeeService
    {
        Task<Employee> CreateEmployee(string name);
        Task<Employee> FindByName(string name);
        Task<Employee> GetById(Guid id);
        Task<List<Employee>> GetAll();
        Task Delete(Guid id);
        Task AddSubordinate(Employee currentEmployee, Employee subordinate);
        Task DeleteSubordinate(Employee currentEmployee, Employee subordinate);
        Task SetSupervisor(Employee currentEmployee, Employee supervisor);
    }
}