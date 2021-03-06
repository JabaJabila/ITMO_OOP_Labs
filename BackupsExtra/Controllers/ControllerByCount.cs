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
using Newtonsoft.Json;

namespace BackupsExtra.Controllers
{
    public class ControllerByCount : IRestorePointController
    {
        [JsonProperty("algorithm")]
        private readonly IRestorePointsCleaningAlgorithm _algorithm;

        public ControllerByCount(int limitAmount, IRestorePointsCleaningAlgorithm algorithm)
        {
            if (limitAmount <= 0)
                throw new BackupException("Impossible to set limit to store <= 0 points maximum!");

            LimitAmount = limitAmount;
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        public int LimitAmount { get; }

        public IReadOnlyCollection<RestorePoint> ControlRestorePoints(
            IReadOnlyList<RestorePoint> backupRestorePoints,
            IExtendedRepository repository,
            ILogger logger,
            IStorageCreationAlgorithm storageCreationAlgorithm)
        {
            if (backupRestorePoints.Count <= LimitAmount) return null;

            long countPointsToRemove = backupRestorePoints.Count - LimitAmount;
            if (countPointsToRemove == backupRestorePoints.Count)
            {
                var exception = new BackupException("Unacceptable operation! " +
                                                    "Restore point controller has to clean all restore points!");
                logger.LogException(exception);
                throw exception;
            }

            var restorePointInfo = new List<string>();
            var pointsSortedCopy = backupRestorePoints.ToList();
            var restorePointsToDelete = new List<RestorePoint>();
            pointsSortedCopy.Sort((x, y)
                => DateTime.Compare(x.CreationTime, y.CreationTime));

            for (int pointListPos = 0;
                pointListPos < countPointsToRemove;
                pointListPos++)
            {
                _algorithm.CleanRestorePoint(
                    pointsSortedCopy[pointListPos],
                    pointsSortedCopy[(Index)countPointsToRemove],
                    repository,
                    storageCreationAlgorithm);
                restorePointInfo.Add("\t" + pointsSortedCopy[pointListPos].RestorePointInfo());
                restorePointsToDelete.Add(pointsSortedCopy[pointListPos]);
            }

            logger.LogMessage($"Cleaned {countPointsToRemove} restore points by {_algorithm.GetType().Name}:\n"
                   + string.Join('\n', restorePointInfo));

            return restorePointsToDelete;
        }
    }
}