using System;
using System.Collections.Generic;
using Backups.Entities;
using Backups.Repository;

namespace Backups.Algorithms
{
    public interface IStorageCreationAlgorithm
    {
        IReadOnlyCollection<Storage> CreateStorages(IRepository repository, List<string> jobObjectPaths, Guid backupJobId);
    }
}