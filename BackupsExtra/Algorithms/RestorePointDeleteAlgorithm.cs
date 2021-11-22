using System.Linq;
using Backups.Algorithms;
using Backups.Entities;
using BackupsExtra.Wrappers.Repositories;

namespace BackupsExtra.Algorithms
{
    public class RestorePointDeleteAlgorithm : IRestorePointsCleaningAlgorithm
    {
        public void CleanRestorePoint(
            RestorePoint pointOverLimit,
            RestorePoint oldestPointInTheLimit,
            IExtendedRepository repository,
            IStorageCreationAlgorithm storageCreationAlgorithm)
        {
            repository.DeleteStorages(pointOverLimit.Storages.Select(storage => storage.FullName).ToList());
        }
    }
}