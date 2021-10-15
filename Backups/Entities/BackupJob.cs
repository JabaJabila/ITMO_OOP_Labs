using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Algorithms;
using Backups.Repository;
using Backups.Tools;

namespace Backups.Entities
{
    public class BackupJob
    {
        private readonly IStorageCreationAlgorithm _algorithm;
        private readonly List<JobObject> _jobObjects;

        public BackupJob(
            IRepository repository,
            IStorageCreationAlgorithm algorithm,
            IReadOnlyCollection<JobObject> jobObjects = null)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
            Id = Guid.NewGuid();
            Repository.CreateBackupJobRepository(Id);
            _jobObjects = new List<JobObject>();

            JobObject invalidJobObject = jobObjects?.FirstOrDefault(jobObject => Repository
                .CheckIfJobObjectExists(jobObject.FullName));

            if (invalidJobObject != null)
            {
                throw new BackupException($"Impossible to create Backup Job! Job object" +
                                          $" {invalidJobObject.FullName} doesn't exist!");
            }

            _jobObjects = jobObjects == null ? new List<JobObject>() : jobObjects.ToList();
            Backup = new Backup();
        }

        public Guid Id { get; }
        public IRepository Repository { get; }
        public IReadOnlyCollection<JobObject> JobObjects => _jobObjects;
        public Backup Backup { get; }

        public void AddJobObject(JobObject jobObject)
        {
            if (!Repository.CheckIfJobObjectExists(jobObject.FullName))
                throw new BackupException($"Job object {jobObject.FullName} doesn't exist!");

            if (_jobObjects.Contains(jobObject))
                throw new BackupException($"{jobObject.FullName} already in this Backup Job!");

            _jobObjects.Add(jobObject);
        }

        public void DeleteJobObject(JobObject jobObject)
        {
            if (!_jobObjects.Remove(jobObject))
                throw new BackupException($"{jobObject.FullName} not in this Backup Job!");
        }

        public void CreateRestorePoint(DateTime? creationTime = null)
        {
            IEnumerable<string> storagePaths = _algorithm.CreateStorages(
                Repository,
                JobObjects.Select(jobObject => jobObject.FullName).ToList(),
                Id);

            Backup.AddRestorePoint(
                new RestorePoint(storagePaths.Select(path => new Storage(path)), creationTime));
        }

        public void DeleteRestorePoint(RestorePoint restorePoint)
        {
            if (Backup.DeleteRestorePoint(restorePoint))
            {
                Repository.DeleteStorages(restorePoint.Storages.Select(storage => storage.FullName).ToList());
            }
        }
    }
}