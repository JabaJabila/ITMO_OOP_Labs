using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using Backups.Tools;
using BackupsExtra.Algorithms;
using BackupsExtra.Extensions;
using BackupsExtra.Loggers;
using BackupsExtra.Wrappers.Repositories;

namespace BackupsExtra.Controllers
{
    public class ControllerByDate : IRestorePointController
    {
        private readonly IRestorePointsCleaningAlgorithm _algorithm;

        public ControllerByDate(DateTime limitDate, IRestorePointsCleaningAlgorithm algorithm)
        {
            LimitDate = limitDate;
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        public DateTime LimitDate { get; }

        public IReadOnlyCollection<RestorePoint> ControlRestorePoints(
            IReadOnlyList<RestorePoint> backupRestorePoints,
            IExtendedRepository repository,
            ILogger logger)
        {
            var restorePointInfo = new List<string>();
            var pointsSortedCopy = backupRestorePoints.ToList();
            var restorePointsToDelete = new List<RestorePoint>();
            pointsSortedCopy.Sort((x, y)
                => DateTime.Compare(x.CreationTime, y.CreationTime));

            pointsSortedCopy
                .Where(point => DateTime.Compare(point.CreationTime, LimitDate) < 0)
                .ToList()
                .ForEach(
                    point =>
                    {
                        restorePointInfo.Add("\t" + point.RestorePointInfo());
                        restorePointsToDelete.Add(point);
                    });

            if (restorePointsToDelete.Count == backupRestorePoints.Count)
            {
                var exception = new BackupException("Unacceptable operation! " +
                                                    "Restore point controller has to clean all restore points!");
                logger.LogException(exception);
                throw exception;
            }

            RestorePoint oldestPointInTheLimit = pointsSortedCopy
                .First(point => DateTime.Compare(point.CreationTime, LimitDate) >= 0);

            restorePointsToDelete.ForEach(point =>
                _algorithm.CleanRestorePoint(point, oldestPointInTheLimit, repository));

            logger.LogMessage($"Cleaned {restorePointsToDelete.Count} restore points by {_algorithm.GetType().Name}:\n"
                                  + string.Join('\n', restorePointInfo));

            return restorePointsToDelete;
        }
    }
}