using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Algorithms;
using Backups.Entities;
using Backups.Tools;
using BackupsExtra.Controllers;
using BackupsExtra.Extensions;
using BackupsExtra.Loggers;
using BackupsExtra.Wrappers.Repositories;

namespace BackupsExtra.Wrappers.BackupJob
{
    public class ExtendedBackupJob
    {
        private readonly Backups.Entities.BackupJob _backupJob;
        private readonly ILogger _logger;
        private readonly IExtendedRepository _repository;
        private IRestorePointController _restorePointController;

        public ExtendedBackupJob(
            IExtendedRepository repository,
            IStorageCreationAlgorithm creationAlgorithm,
            ILogger logger,
            IRestorePointController controller,
            IReadOnlyCollection<JobObject> jobObjects = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _restorePointController = controller ?? throw new ArgumentNullException(nameof(controller));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _backupJob = new Backups.Entities.BackupJob(repository, creationAlgorithm, jobObjects);
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
                _logger.LogMessage($"Created {GetOldestRestorePoint().RestorePointInfo()}");
                _restorePointController
                    .ControlRestorePoints(Backup.RestorePoints, _repository, _logger)
                    .ToList()
                    .ForEach(DeleteRestorePoint);
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

        public void RestoreToOriginalLocation(RestorePoint restorePoint)
        {
            if (restorePoint == null)
                throw new ArgumentNullException(nameof(restorePoint));

            if (!Backup.RestorePoints.Contains(restorePoint))
            {
                var exception = new BackupException($"Restore point {restorePoint.Id} " +
                                                          "wasn't created by this backup job!");
                _logger.LogException(exception);
                throw exception;
            }

            try
            {
                _repository.RestoreToOriginalLocation(
                    restorePoint.Storages.Select(storage => storage.FullName).ToList());
                _logger.LogMessage($"Successfully restored to original location {restorePoint.RestorePointInfo()}");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                throw;
            }
        }

        public void RestoreToDifferentLocation(RestorePoint restorePoint, string path)
        {
            if (restorePoint == null)
                throw new ArgumentNullException(nameof(restorePoint));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!Backup.RestorePoints.Contains(restorePoint))
            {
                var exception = new BackupException($"Restore point {restorePoint.Id} " +
                                                    "wasn't created by this backup job!");
                _logger.LogException(exception);
                throw exception;
            }

            try
            {
                _repository.RestoreToDifferentLocation(
                    restorePoint.Storages.Select(storage => storage.FullName).ToList(),
                    path);
                _logger.LogMessage($"Successfully restored to {path} {restorePoint.RestorePointInfo()}");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                throw;
            }
        }

        public void ChangeRestorePointController(IRestorePointController newController)
        {
            _restorePointController = newController ?? throw new ArgumentNullException(nameof(newController));
            _logger.LogMessage($"Changed restore point controller at {_backupJob.BackupJobInfo()}");
            _restorePointController.ControlRestorePoints(Backup.RestorePoints, _repository, _logger);
        }

        private RestorePoint GetOldestRestorePoint()
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