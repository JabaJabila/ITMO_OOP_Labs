using Core.Domain.Entities.TaskChanges;

namespace Core.RepositoryAbstractions
{
    public interface ITaskChangesRepository : IRepository<JobTaskChange>
    {
    }
}