using System.Collections.Generic;
using Backups.Entities;

namespace BackupsExtra.Limits
{
    public interface IRestorePointLimiter
    {
        void ControlRestorePoints(IReadOnlyList<RestorePoint> backupRestorePoints);
    }
}