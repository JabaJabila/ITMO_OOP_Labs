using System;
using System.Collections.Generic;
using Backups.Repository;

namespace Backups.Algorithms
{
    public interface IStorageCreationAlgorithm
    {
        IEnumerable<string> CreateStorages(IRepository repository, List<string> jobObjectPaths, Guid backupJobId);
    }
}