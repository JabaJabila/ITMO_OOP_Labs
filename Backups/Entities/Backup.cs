using System;
using System.Collections.Generic;
using Backups.Tools;

namespace Backups.Entities
{
    public class Backup
    {
        private readonly List<RestorePoint> _restorePoints;

        internal Backup()
        {
            _restorePoints = new List<RestorePoint>();
        }

        public IReadOnlyList<RestorePoint> RestorePoints => _restorePoints;

        internal void AddRestorePoint(RestorePoint restorePoint)
        {
            if (restorePoint == null)
                throw new ArgumentNullException(nameof(restorePoint));

            if (_restorePoints.Contains(restorePoint))
                throw new BackupException($"Restore Point {restorePoint.Id.ToString()} already created!");

            _restorePoints.Add(restorePoint);
        }

        internal bool DeleteRestorePoint(RestorePoint restorePoint)
        {
            return restorePoint != null && _restorePoints.Remove(restorePoint);
        }
    }
}