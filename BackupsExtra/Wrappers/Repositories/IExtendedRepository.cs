using System.Collections.Generic;
using Backups.Repository;

namespace BackupsExtra.Wrappers.Repositories
{
    public interface IExtendedRepository : IRepository
    {
        bool CheckIfStorageInRestorePoint(string storageFullName, List<string> storagePathsInPoint);
        void RestoreToDifferentLocation(IReadOnlyCollection<string> storagePaths, string pathToRestore);
        void RestoreToOriginalLocation(IReadOnlyCollection<string> storagePaths);
    }
}