using System;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.RepositoryAbstractions
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task Delete(Guid id);
    }
}