using System.Linq;
using Backups.Entities;
using BackupsExtra.Wrappers;
using BackupsExtra.Wrappers.Repositories;

namespace BackupsExtra.Algorithms
{
    public class RestorePointDeleteAlgorithm : IRestorePointsCleaningAlgorithm
    {
        public void CleanRestorePoint(
            RestorePoint pointOverLimit,
            RestorePoint oldestPointInTheLimit,
            IExtendedRepository repository)
        {
            repository.DeleteStorages(pointOverLimit.Storages.Select(storage => storage.FullName).ToList());
        }
    }
}