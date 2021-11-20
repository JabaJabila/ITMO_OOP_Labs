using Backups.Entities;
using BackupsExtra.Wrappers;

namespace BackupsExtra.Algorithms
{
    public interface IRestorePointsCleaningAlgorithm
    {
        void CleanRestorePoint(
            RestorePoint pointOverLimit,
            RestorePoint oldestPointInTheLimit,
            IExtendedRepository repository);
    }
}