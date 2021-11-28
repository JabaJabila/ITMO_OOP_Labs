using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Backups.Repository
{
    public class LocalMemoryRepository : IRepository
    {
        [JsonProperty("memoryRepositoryData")]
        private Dictionary<string, List<string>> _memoryRepositoryData;

        public LocalMemoryRepository()
        {
        }

        [JsonConstructor]
        private LocalMemoryRepository(Dictionary<string, List<string>> memoryRepositoryData)
        {
            _memoryRepositoryData = memoryRepositoryData ??
                                    throw new ArgumentNullException(nameof(memoryRepositoryData));
        }

        public void CreateBackupJobRepository(Guid backupJobId)
        {
            _memoryRepositoryData = new Dictionary<string, List<string>>();
        }

        public bool CheckIfJobObjectExists(string fullName)
        {
            return File.Exists(fullName);
        }

        public string CreateStorage(List<string> jobObjectsPaths, Guid backupJobId, Guid storageId)
        {
            if (jobObjectsPaths == null)
                throw new ArgumentNullException(nameof(jobObjectsPaths));

            string storagePath = Path.Combine(
                backupJobId.ToString(),
                storageId.ToString());

            _memoryRepositoryData[storagePath] = jobObjectsPaths.Select(File.ReadAllText).ToList();

            return storagePath;
        }

        public string CreateStorage(string jobObjectPath, Guid backupJobId, Guid storageId)
        {
            if (jobObjectPath == null)
                throw new ArgumentNullException(nameof(jobObjectPath));

            string storagePath = Path.Combine(
                backupJobId.ToString(),
                storageId.ToString());

            _memoryRepositoryData[storagePath] = new List<string>(new[] { File.ReadAllText(jobObjectPath) });

            return storagePath;
        }

        public void DeleteStorages(List<string> storagesNames)
        {
            storagesNames.ForEach(path => _memoryRepositoryData.Remove(path));
        }

        public List<string> ReadFromStorage(string storagePath)
        {
            return _memoryRepositoryData.ContainsKey(storagePath) ?
                _memoryRepositoryData[storagePath].ToList() : null;
        }
    }
}