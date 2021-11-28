using System.Linq;
using Backups.Algorithms;
using Backups.Entities;
using BackupsExtra.Wrappers.Repositories;

namespace BackupsExtra.Algorithms
{
    public class RestorePointMergeAlgorithm : IRestorePointsCleaningAlgorithm
    {
        private const string StorageAlgorithmExclusion = "SingleStorageAlgorithm";

        public void CleanRestorePoint(
            RestorePoint pointOverLimit,
            RestorePoint oldestPointInTheLimit,
            IExtendedRepository repository,
            IStorageCreationAlgorithm storageCreationAlgorithm)
        {
            if (storageCreationAlgorithm.GetType().Name == StorageAlgorithmExclusion)
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

            newStorages.ForEach(storage => oldestPointInTheLimit.RelocateStorage(storage, pointOverLimit));
        }
    }
}