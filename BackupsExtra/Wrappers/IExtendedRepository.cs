using System.Collections.Generic;
using Backups.Entities;
using Backups.Repository;

namespace BackupsExtra.Wrappers
{
    public interface IExtendedRepository : IRepository
    {
        bool IsSingleStorageType(RestorePoint oldestPointInTheLimit);
        bool CheckIfStorageInRestorePoint(string storageFullName, List<string> toList);
        void RestoreToDifferentLocation(IEnumerable<string> storagePaths, string path);
        void RestoreToOriginalLocation(IEnumerable<string> storagePaths);
    }
}