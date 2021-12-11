using System;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Domain.ServicesAbstractions
{
    public interface IEmployeeService
    {
        Task<Employee> Create(string name);

        Task<Employee> FindByName(string name);

        Task<Employee> FindById(Guid id);

        void Delete(Guid id);

        Employee Update(Employee entity);
    }
}