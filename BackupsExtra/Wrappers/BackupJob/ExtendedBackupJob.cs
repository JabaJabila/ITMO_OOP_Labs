using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Backups.Algorithms;
using Backups.Entities;
using Backups.Tools;
using BackupsExtra.Controllers;
using BackupsExtra.Extensions;
using BackupsExtra.Loggers;
using BackupsExtra.Wrappers.Repositories;
using Newtonsoft.Json;

namespace BackupsExtra.Wrappers.BackupJob
{
    public class ExtendedBackupJob
    {
        [JsonProperty("backupJob")]
        private readonly Backups.Entities.BackupJob _backupJob;
        [JsonProperty("logger")]
        private readonly ILogger _logger;
        [JsonProperty("repository")]
        private readonly IExtendedRepository _repository;
        [JsonProperty("creationAlgorithm")]
        private readonly IStorageCreationAlgorithm _creationAlgorithm;
        [JsonProperty("restorePointController")]
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
            _creationAlgorithm = creationAlgorithm ?? throw new ArgumentNullException(nameof(creationAlgorithm));
            _backupJob = new Backups.Entities.BackupJob(repository, creationAlgorithm, jobObjects);
            _logger.LogMessage($"Created {_backupJob.BackupJobInfo()}");
        }

        [JsonConstructor]
        private ExtendedBackupJob(
            Backups.Entities.BackupJob backupJob,
            IExtendedRepository repository,
            IStorageCreationAlgorithm creationAlgorithm,
            ILogger logger,
            IRestorePointController restorePointController)
        {
            _backupJob = backupJob ?? throw new ArgumentNullException(nameof(backupJob));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _creationAlgorithm = creationAlgorithm ?? throw new ArgumentNullException(nameof(creationAlgorithm));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _restorePointController =
                restorePointController ?? throw new ArgumentNullException(nameof(restorePointController));
            _logger.LogMessage($"Successfully loaded backup jon {_backupJob.BackupJobInfo()}");
        }

        [JsonIgnore]
        public Guid Id => _backupJob.Id;
        [JsonIgnore]
        public IReadOnlyCollection<JobObject> JobObjects => _backupJob.JobObjects;
        [JsonIgnore]
        public Backup Backup => _backupJob.Backup;

        public static ExtendedBackupJob LoadBackupJob(FileStream stream)
        {
            if (!stream.CanRead) throw new BackupException("Impossible to load data! Stream closed for reading!");

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] data = memoryStream.ToArray();

            ExtendedBackupJob backupJob = JsonConvert
                .DeserializeObject<ExtendedBackupJob>(
                    Encoding.UTF8.GetString(data),
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                    });

            return backupJob;
        }

        public void SaveBackupJob(FileStream stream)
        {
            try
            {
                if (!stream.CanWrite)
                    throw new BackupException("Impossible to save data! Stream closed for writing!");

                string serializedData = JsonConvert.SerializeObject(
                    this,
                    Formatting.Indented,
                    new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                stream.Write(Encoding.UTF8.GetBytes(serializedData));
                _logger.LogMessage($"Successfully saved backup job's {_backupJob.BackupJobInfo()} state");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
                throw;
            }
        }

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
                IReadOnlyCollection<RestorePoint> pointsToDelete = _restorePointController
                    .ControlRestorePoints(Backup.RestorePoints, _repository, _logger, _creationAlgorithm);

                pointsToDelete?.ToList().ForEach(_backupJob.DeleteRestorePoint);
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
            IReadOnlyCollection<RestorePoint> pointsToDelete = _restorePointController
                .ControlRestorePoints(Backup.RestorePoints, _repository, _logger, _creationAlgorithm);

            pointsToDelete?.ToList().ForEach(_backupJob.DeleteRestorePoint);
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