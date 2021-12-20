using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.RepositoryAbstractions;

namespace Infrastructure.Repositories
{
    public class JobTaskRepository : IJobTaskRepository
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<JobTask>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<JobTask> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<JobTask> Add(JobTask entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(JobTask entity)
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