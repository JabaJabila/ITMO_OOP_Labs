using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.RepositoryAbstractions
{
    public interface IRepository<T> : IDisposable
    {
        Task<IReadOnlyCollection<T>> GetAll();
        Task<T> GetById(Guid id);
        Task<T> Add(T entity);
        Task SaveChanges();
    }
}