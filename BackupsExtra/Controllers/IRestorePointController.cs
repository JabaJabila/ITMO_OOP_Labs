using System.Collections.Generic;
using Backups.Entities;
using BackupsExtra.Loggers;
using BackupsExtra.Wrappers;

namespace BackupsExtra.Controllers
{
    public interface IRestorePointController
    {
        IReadOnlyCollection<RestorePoint> ControlRestorePoints(
            IReadOnlyList<RestorePoint> backupRestorePoints,
            IExtendedRepository repository,
            ILogger logger);
    }
}