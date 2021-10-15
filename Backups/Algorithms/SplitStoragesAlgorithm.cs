using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Repository;
using Backups.Tools;

namespace Backups.Algorithms
{
    public class SplitStoragesAlgorithm : IStorageCreationAlgorithm
    {
        public IEnumerable<string> CreateStorages(
            IRepository repository,
            List<string> jobObjectPaths,
            Guid backupJobId)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            if (jobObjectPaths.Count == 0)
                throw new BackupException("Impossible to create Restore point! No Job Objects added!");

            return jobObjectPaths
                .Select(jobObjectPath =>
                    repository.CreateStorage(jobObjectPath, backupJobId, Guid.NewGuid()));
        }
    }
}