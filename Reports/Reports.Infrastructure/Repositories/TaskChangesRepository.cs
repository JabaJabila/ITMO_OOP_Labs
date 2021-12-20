using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities.TaskChanges;
using Core.RepositoryAbstractions;

namespace Infrastructure.Repositories
{
    public class TaskChangesRepository : ITaskChangesRepository
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<JobTaskChange>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<JobTaskChange> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<JobTaskChange> Add(JobTaskChange entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(JobTaskChange entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}