using System;
using System.Collections.Generic;
using Backups.Repository;
using Backups.Tools;

namespace Backups.Algorithms
{
    public class SingleStorageAlgorithm : IStorageCreationAlgorithm
    {
        public IEnumerable<string> CreateStorages(IRepository repository, List<string> jobObjectPaths, Guid backupJobId)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            if (jobObjectPaths.Count == 0)
                throw new BackupException("Impossible to create Restore point! No Job Objects added!");

            var storageId = Guid.NewGuid();

            var storagePathList =
                new List<string>(new[]
                    {
                        repository.CreateStorage(jobObjectPaths, backupJobId, storageId),
                    });

            return storagePathList;
        }
    }
}