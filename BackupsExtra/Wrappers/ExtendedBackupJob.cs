using System;
using System.Collections.Generic;
using Backups.Algorithms;
using Backups.Entities;
using Backups.Repository;
using BackupsExtra.Algorithms;
using BackupsExtra.Limits;
using BackupsExtra.Loggers;

namespace BackupsExtra.Wrappers
{
    public class ExtendedBackupJob
    {
        private readonly BackupJob _backupJob;
        private readonly ILogger _logger;
        private readonly IStorageCleaningAlgorithm _cleaningAlgorithm;
        private readonly IRestorePointLimit _restorePointLimit;

        public ExtendedBackupJob(
            IRepository repository,
            IStorageCreationAlgorithm creationAlgorithm,
            ILogger logger,
            IStorageCleaningAlgorithm cleaningAlgorithm,
            IRestorePointLimit limit,
            IReadOnlyCollection<JobObject> jobObjects = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cleaningAlgorithm = cleaningAlgorithm ?? throw new ArgumentNullException(nameof(cleaningAlgorithm));
            _restorePointLimit = limit ?? throw new ArgumentNullException(nameof(limit));
            _backupJob = new BackupJob(repository, creationAlgorithm, jobObjects);
            _logger.LogMessage();
        }

        public Guid Id => _backupJob.Id;
        public IReadOnlyCollection<JobObject> JobObjects => _backupJob.JobObjects;
        public Backup Backup => _backupJob.Backup;

        public void AddJobObject(JobObject jobObject)
        {
            try
            {
                _backupJob.AddJobObject(jobObject);
                _logger.LogMessage(" "); // TODO
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
                _logger.LogMessage(" "); // TODO
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
                _logger.LogMessage(" "); // TODO
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
                _logger.LogMessage(" "); // TODO
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                throw;
            }
        }
    }
}