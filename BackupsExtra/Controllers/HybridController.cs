using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Algorithms;
using Backups.Entities;
using Backups.Tools;
using BackupsExtra.Algorithms;
using BackupsExtra.Extensions;
using BackupsExtra.Loggers;
using BackupsExtra.Wrappers.Repositories;

namespace BackupsExtra.Controllers
{
    public class HybridController : IRestorePointController
    {
        private readonly IRestorePointsCleaningAlgorithm _algorithm;

        public HybridController(
            int limitAmount,
            DateTime limitDate,
            IRestorePointsCleaningAlgorithm algorithm,
            bool cleanIfBothConditionsFitted = true)
        {
            LimitDate = limitDate;
            if (limitAmount <= 0)
                throw new BackupException("Impossible to set limit to store <= 0 points maximum!");

            LimitAmount = limitAmount;
            CleanIfBothConditionsFitted = cleanIfBothConditionsFitted;
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        public int LimitAmount { get; }
        public DateTime LimitDate { get; }
        public bool CleanIfBothConditionsFitted { get; }

        public IReadOnlyCollection<RestorePoint> ControlRestorePoints(
            IReadOnlyList<RestorePoint> backupRestorePoints,
            IExtendedRepository repository,
            ILogger logger,
            IStorageCreationAlgorithm storageCreationAlgorithm)
        {
            var pointsSortedCopy = backupRestorePoints.ToList();
            var restorePointsToDeleteByCount = new List<RestorePoint>();
            var restorePointsToDeleteByDate = new List<RestorePoint>();
            pointsSortedCopy.Sort((x, y)
                => DateTime.Compare(x.CreationTime, y.CreationTime));

            for (int pointListPos = 0;
                pointListPos < pointsSortedCopy.Count - LimitAmount;
                pointListPos++)
            {
                restorePointsToDeleteByCount.Add(pointsSortedCopy[pointListPos]);
            }

            pointsSortedCopy
                .Where(point => DateTime.Compare(point.CreationTime, LimitDate) < 0)
                .ToList()
                .ForEach(point => restorePointsToDeleteByDate.Add(point));

            List<RestorePoint> restorePointsToDelete = CleanIfBothConditionsFitted
                ? restorePointsToDeleteByCount.Intersect(restorePointsToDeleteByDate).ToList()
                : restorePointsToDeleteByCount.Union(restorePointsToDeleteByDate).ToList();

            if (restorePointsToDelete.Count == backupRestorePoints.Count)
            {
                var exception = new BackupException("Unacceptable operation! " +
                                                    "Restore point controller has to clean all restore points!");
                logger.LogException(exception);
                throw exception;
            }

            var restorePointInfo = restorePointsToDelete
                .Select(point => "\t" + point.RestorePointInfo())
                .ToList();

            RestorePoint oldestPointInTheLimit = pointsSortedCopy
                .First(point => !restorePointsToDelete.Contains(point));

            restorePointsToDelete.ForEach(point =>
                _algorithm.CleanRestorePoint(point, oldestPointInTheLimit, repository, storageCreationAlgorithm));

            logger.LogMessage($"Cleaned {restorePointsToDelete.Count} restore points by {_algorithm.GetType().Name}:\n"
                                  + string.Join('\n', restorePointInfo));

            return restorePointsToDelete;
        }
    }
}