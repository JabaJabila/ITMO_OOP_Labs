using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Repository;
using Backups.Tools;
using BackupsExtra.Wrappers.Compressors;

namespace BackupsExtra.Wrappers.Repositories
{
    public class ExtendedLocalFilesRepository : IExtendedRepository, IRepositoryWithArchivator
    {
        private readonly LocalFilesRepository _repository;
        private readonly Dictionary<string, List<string>> _objectsOriginalLocation;
        private readonly IExtendedCompressor _compressor;

        public ExtendedLocalFilesRepository(
            string repositoryPath,
            IExtendedCompressor compressor,
            string storageFileExtension)
        {
            _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
            _repository = new LocalFilesRepository(repositoryPath, compressor, storageFileExtension);
            _objectsOriginalLocation = new Dictionary<string, List<string>>();
        }

        public void CreateBackupJobRepository(Guid backupJobId)
        {
            _repository.CreateBackupJobRepository(backupJobId);
        }

        public bool CheckIfJobObjectExists(string fullName)
        {
            return _repository.CheckIfJobObjectExists(fullName);
        }

        public string CreateStorage(List<string> jobObjectsPaths, Guid backupJobId, Guid storageId)
        {
            string storagePath = _repository.CreateStorage(jobObjectsPaths, backupJobId, storageId);
            _objectsOriginalLocation[storagePath] = jobObjectsPaths;
            return storagePath;
        }

        public string CreateStorage(string jobObjectPath, Guid backupJobId, Guid storageId)
        {
            string storagePath = _repository.CreateStorage(jobObjectPath, backupJobId, storageId);
            _objectsOriginalLocation[storagePath] = new List<string> { jobObjectPath };
            return storagePath;
        }

        public void DeleteStorages(List<string> storagesNames)
        {
            _repository.DeleteStorages(storagesNames);
            storagesNames.ForEach(name => _objectsOriginalLocation.Remove(name));
        }

        public void SaveInArchive(string storagePath, string jobObjectPath)
        {
            _repository.SaveInArchive(storagePath, jobObjectPath);
        }

        public bool IsSingleStorageType(IReadOnlyCollection<string> storagePaths)
        {
            if (storagePaths == null)
                throw new ArgumentNullException(nameof(storagePaths));

            return !storagePaths.ToList().All(path => _compressor.IfStorageContainsOneObject(path));
        }

        public bool CheckIfStorageInRestorePoint(string storageFullName, List<string> storagePathsInPoint)
        {
            if (storageFullName == null)
                throw new ArgumentNullException(nameof(storageFullName));
            if (storagePathsInPoint == null)
                throw new ArgumentNullException(nameof(storagePathsInPoint));

            if (!_objectsOriginalLocation.ContainsKey(storageFullName)) return false;
            List<string> objectPaths = _objectsOriginalLocation[storageFullName];

            return storagePathsInPoint
                .Where(_ => _objectsOriginalLocation.ContainsKey(storageFullName))
                .Any(storagePath => objectPaths.Intersect(_objectsOriginalLocation[storagePath])
                    .ToList().Count == objectPaths.Count);
        }

        public void RestoreToDifferentLocation(IReadOnlyCollection<string> storagePaths, string pathToRestore)
        {
            if (storagePaths == null)
                throw new ArgumentNullException(nameof(storagePaths));

            if (pathToRestore == null)
                throw new ArgumentNullException(nameof(pathToRestore));

            foreach (string storagePath in storagePaths)
            {
                List<string> objectPaths = _objectsOriginalLocation[storagePath];
                foreach (string objectPath in objectPaths)
                {
                    if (File.Exists(Path.Combine(pathToRestore, Path.GetFileName(objectPath))))
                    {
                        throw new BackupException($"Impossible to restore to {pathToRestore}" +
                                                  $"file {Path.GetFileName(objectPath)} already exists");
                    }

                    _compressor.Extract(storagePath, Path.GetFileName(objectPath), pathToRestore);
                }
            }
        }

        public void RestoreToOriginalLocation(IReadOnlyCollection<string> storagePaths)
        {
            if (storagePaths == null)
                throw new ArgumentNullException(nameof(storagePaths));

            storagePaths.ToList().ForEach(storagePath =>
            {
                List<string> objectPaths = _objectsOriginalLocation[storagePath];
                objectPaths.ForEach(objectPath =>
                    _compressor.Extract(storagePath, Path.GetFileName(objectPath), objectPath));
            });
        }
    }
}