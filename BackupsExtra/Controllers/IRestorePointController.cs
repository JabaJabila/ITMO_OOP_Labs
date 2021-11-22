using System.Collections.Generic;
using Backups.Algorithms;
using Backups.Entities;
using BackupsExtra.Loggers;
using BackupsExtra.Wrappers.Repositories;

namespace BackupsExtra.Controllers
{
    public interface IRestorePointController
    {
        IReadOnlyCollection<RestorePoint> ControlRestorePoints(
            IReadOnlyList<RestorePoint> backupRestorePoints,
            IExtendedRepository repository,
            ILogger logger,
            IStorageCreationAlgorithm storageCreationAlgorithm);
    }
}