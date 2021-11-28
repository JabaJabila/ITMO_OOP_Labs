using Backups.Algorithms;
using Backups.Entities;
using BackupsExtra.Wrappers.Repositories;

namespace BackupsExtra.Algorithms
{
    public interface IRestorePointsCleaningAlgorithm
    {
        void CleanRestorePoint(
            RestorePoint pointOverLimit,
            RestorePoint oldestPointInTheLimit,
            IExtendedRepository repository,
            IStorageCreationAlgorithm storageCreationAlgorithm);
    }
}