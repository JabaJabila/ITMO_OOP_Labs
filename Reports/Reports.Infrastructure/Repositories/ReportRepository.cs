using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.RepositoryAbstractions;

namespace Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Report>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Report> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Report> Add(Report entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(Report entity)
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