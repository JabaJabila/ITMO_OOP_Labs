using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Repository;
using Backups.Tools;
using Newtonsoft.Json;

namespace BackupsExtra.Wrappers.Repositories
{
    public class ExtendedLocalMemoryRepository : IExtendedRepository
    {
        [JsonProperty("repository")]
        private readonly LocalMemoryRepository _repository;
        [JsonProperty("objectsOriginalLocation")]
        private readonly Dictionary<string, List<string>> _objectsOriginalLocation;

        public ExtendedLocalMemoryRepository()
        {
            _repository = new LocalMemoryRepository();
            _objectsOriginalLocation = new Dictionary<string, List<string>>();
        }

        [JsonConstructor]
        private ExtendedLocalMemoryRepository(
            LocalMemoryRepository repository,
            Dictionary<string, List<string>> objectsOriginalLocation)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _objectsOriginalLocation = objectsOriginalLocation ??
                                       throw new ArgumentNullException(nameof(objectsOriginalLocation));
        }

        public void CreateBackupJobRepository(Guid backupJobId)
            => _repository.CreateBackupJobRepository(backupJobId);

        public bool CheckIfJobObjectExists(string fullName)
            => _repository.CheckIfJobObjectExists(fullName);

        public string CreateStorage(List<string> jobObjectsPaths, Guid backupJobId, Guid storageId)
        {
            string storagePath = _repository.CreateStorage(jobObjectsPaths, backupJobId, storageId);
            _objectsOriginalLocation[storagePath] = jobObjectsPaths.ToList();
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

            Directory.CreateDirectory(pathToRestore);

            storagePaths.ToList().ForEach(storagePath =>
            {
                List<string> objectPaths = _objectsOriginalLocation[storagePath];
                for (int objCounter = 0; objCounter < objectPaths.Count; objCounter++)
                {
                    string filename = Path.GetFileName(objectPaths[objCounter]);
                    string extractTo = Path.Combine(pathToRestore, Path.GetFileName(objectPaths[objCounter]));
                    if (File.Exists(extractTo))
                    {
                        throw new BackupException($"Impossible to restore to {pathToRestore}" +
                                                  $"file {filename} already exists");
                    }

                    using StreamWriter sw = File.CreateText(extractTo);
                    sw.WriteLine(string.Join('\n', _repository.ReadFromStorage(storagePath)[objCounter]));
                }
            });
        }

        public void RestoreToOriginalLocation(IReadOnlyCollection<string> storagePaths)
        {
            if (storagePaths == null)
                throw new ArgumentNullException(nameof(storagePaths));

            storagePaths.ToList().ForEach(storagePath =>
            {
                List<string> objectPaths = _objectsOriginalLocation[storagePath];
                foreach (string objPath in objectPaths)
                {
                    using StreamWriter sw = File.CreateText(objPath);
                    sw.WriteLine(string.Join('\n', _repository.ReadFromStorage(storagePath)));
                }
            });
        }

        public List<string> ReadFromStorage(string storagePath)
            => _repository.ReadFromStorage(storagePath);
    }
}