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
        Task<IReadOnlyCollection<Employee>> GetAll();
        Task<Employee> Delete(Guid id);
        Task<Employee> SetSupervisor(Guid currentEmployeeId, Guid supervisorId);
    }
}