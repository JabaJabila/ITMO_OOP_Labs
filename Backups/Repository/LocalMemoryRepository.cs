using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backups.Repository
{
    public class LocalMemoryRepository : IRepository
    {
        private Dictionary<string, List<string>> _memoryRepository;

        public void CreateBackupJobRepository(Guid backupJobId)
        {
            _memoryRepository = new Dictionary<string, List<string>>();
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

            _memoryRepository[storagePath] = jobObjectsPaths.Select(File.ReadAllText).ToList();

            return storagePath;
        }

        public string CreateStorage(string jobObjectPath, Guid backupJobId, Guid storageId)
        {
            if (jobObjectPath == null)
                throw new ArgumentNullException(nameof(jobObjectPath));

            string storagePath = Path.Combine(
                backupJobId.ToString(),
                storageId.ToString());

            _memoryRepository[storagePath] = new List<string>(new[] { File.ReadAllText(jobObjectPath) });

            return storagePath;
        }

        public void DeleteStorages(List<string> storagesNames)
        {
            storagesNames.ForEach(path => _memoryRepository.Remove(path));
        }

        public List<string> ReadFromStorage(string storagePath)
        {
            return _memoryRepository.ContainsKey(storagePath) ?
                _memoryRepository[storagePath].ToList() : null;
        }
    }
}