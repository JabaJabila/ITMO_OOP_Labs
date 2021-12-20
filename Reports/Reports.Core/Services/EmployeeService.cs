using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.ServicesAbstractions;
using Core.Domain.Tools;
using Core.RepositoryAbstractions;

namespace Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        
        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        } 
        
        public async Task<Employee> CreateEmployee(string name)
        {
            var employee = new Employee(name);
            return await _repository.Add(employee);
        }

        public async Task<Employee> FindByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            
            return await _repository.FindByName(name);
        }

        public async Task<Employee> GetById(Guid id)
        {
            return await _repository.GetById(id);
        }

        public async Task<IReadOnlyCollection<Employee>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Employee> Delete(Guid id)
        {
            Employee employeeToDelete = await _repository.GetById(id);
            if (employeeToDelete == null)
                return null;

            if (employeeToDelete.SupervisorId != null)
            {
                Employee supervisor = await _repository.GetById((Guid)employeeToDelete.SupervisorId);
                if (supervisor == null)
                    throw new ReportsAppException("Employee has unknown supervisor. Data can be corrupted!");

            }

            await _repository.Delete(id);
            await _repository.SaveChanges();
            return employeeToDelete;
        }

        public async Task<Employee> SetSupervisor(Guid currentEmployeeId, Guid supervisorId)
        {
            Employee currentEmployee = await _repository.GetById(currentEmployeeId);
            Employee supervisor = await _repository.GetById(supervisorId);

            if (currentEmployee == null)
                throw new ReportsAppException($"Employee {currentEmployeeId} not found!");
            if (supervisor == null)
                throw new ReportsAppException($"Employee {supervisorId} not found!");

            currentEmployee.SupervisorId = supervisorId;
            await _repository.SaveChanges();
            return currentEmployee;
        }
    }
}