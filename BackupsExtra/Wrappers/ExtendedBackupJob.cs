using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Algorithms;
using Backups.Entities;
using Backups.Repository;
using Backups.Tools;
using BackupsExtra.Algorithms;
using BackupsExtra.Extensions;
using BackupsExtra.Limits;
using BackupsExtra.Loggers;

namespace BackupsExtra.Wrappers
{
    public class ExtendedBackupJob
    {
        private readonly BackupJob _backupJob;
        private readonly ILogger _logger;
        private readonly IStorageCleaningAlgorithm _cleaningAlgorithm;
        private readonly IRestorePointLimiter _restorePointLimiter;

        public ExtendedBackupJob(
            IRepository repository,
            IStorageCreationAlgorithm creationAlgorithm,
            ILogger logger,
            IStorageCleaningAlgorithm cleaningAlgorithm,
            IRestorePointLimiter limiter,
            IReadOnlyCollection<JobObject> jobObjects = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cleaningAlgorithm = cleaningAlgorithm ?? throw new ArgumentNullException(nameof(cleaningAlgorithm));
            _restorePointLimiter = limiter ?? throw new ArgumentNullException(nameof(limiter));
            _backupJob = new BackupJob(repository, creationAlgorithm, jobObjects);
            _logger.LogMessage($"Created {_backupJob.BackupJobInfo()}");
        }

        public Guid Id => _backupJob.Id;
        public IReadOnlyCollection<JobObject> JobObjects => _backupJob.JobObjects;
        public Backup Backup => _backupJob.Backup;

        public void AddJobObject(JobObject jobObject)
        {
            try
            {
                _backupJob.AddJobObject(jobObject);
                _logger.LogMessage($"Added {jobObject.JobObjectInfo()}");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                throw;
            }
        }

        public void DeleteJobObject(JobObject jobObject)
        {
            try
            {
                _backupJob.DeleteJobObject(jobObject);
                _logger.LogMessage($"Deleted {jobObject.JobObjectInfo()}");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                throw;
            }
        }

        public void CreateRestorePoint(DateTime? creationTime = null)
        {
            try
            {
                _backupJob.CreateRestorePoint(creationTime);
                _logger.LogMessage($"Created {GetLastRestorePoint().RestorePointInfo()}");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                throw;
            }
        }

        public void DeleteRestorePoint(RestorePoint restorePoint)
        {
            try
            {
                _backupJob.DeleteRestorePoint(restorePoint);
                _logger.LogMessage($"Deleted {restorePoint.RestorePointInfo()}");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                throw;
            }
        }

        private RestorePoint GetLastRestorePoint()
        {
            if (Backup.RestorePoints.Count == 0)
                throw new BackupException("Impossible to get last restore point: no restore points created!");

            var allPoints = Backup.RestorePoints.ToList();
            allPoints.Sort((x, y)
                => DateTime.Compare(x.CreationTime, y.CreationTime));
            return allPoints.Last();
        }
    }
}