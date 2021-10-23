using System;
using System.Collections.Generic;

namespace Backups.Repository
{
    public interface IRepository
    {
        void CreateBackupJobRepository(Guid backupJobId);
        bool CheckIfJobObjectExists(string fullName);
        string CreateStorage(List<string> jobObjectsPaths, Guid backupJobId, Guid storageId);
        string CreateStorage(string jobObjectPath, Guid backupJobId, Guid storageId);
        void DeleteStorages(List<string> storagesNames);
    }
}