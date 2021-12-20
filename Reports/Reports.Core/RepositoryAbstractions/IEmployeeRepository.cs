using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.RepositoryAbstractions
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee> FindByName(string name);
    }
}