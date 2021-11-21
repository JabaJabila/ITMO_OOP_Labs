using System.Linq;
using Backups.Entities;
using BackupsExtra.Wrappers;

namespace BackupsExtra.Algorithms
{
    public class RestorePointMergeAlgorithm : IRestorePointsCleaningAlgorithm
    {
        public void CleanRestorePoint(
            RestorePoint pointOverLimit,
            RestorePoint oldestPointInTheLimit,
            IExtendedRepository repository)
        {
            if (repository.IsSingleStorageType(oldestPointInTheLimit))
            {
                repository.DeleteStorages(pointOverLimit.Storages.Select(storage => storage.FullName).ToList());
                return;
            }

            var newStorages = pointOverLimit.Storages
                .Where(storage =>
                    !repository.CheckIfStorageInRestorePoint(
                        storage.FullName,
                        oldestPointInTheLimit.Storages.Select(s => s.FullName).ToList()))
                .ToList();

            newStorages.ForEach(oldestPointInTheLimit.AddStorage);
        }
    }
}