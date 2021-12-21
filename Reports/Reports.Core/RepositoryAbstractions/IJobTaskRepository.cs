using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.RepositoryAbstractions
{
    public interface IJobTaskRepository : IRepository<JobTask>
    {
    }
}