using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Backups.Repository
{
    public class LocalFilesRepository : IRepositoryWithArchivator
    {
        [JsonProperty("storageFileExtension")]
        private readonly string _storageFileExtension;
        [JsonProperty("compressor")]
        private readonly ICompressor _compressor;
        public LocalFilesRepository(string repositoryPath, ICompressor compressor, string storageFileExtension)
        {
            Directory.CreateDirectory(repositoryPath);
            RepositoryPath = repositoryPath;
            _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
            _storageFileExtension = storageFileExtension ??
                                    throw new ArgumentNullException(nameof(storageFileExtension));
        }

        public string RepositoryPath { get; }

        public void CreateBackupJobRepository(Guid backupJobId)
        {
            Directory.CreateDirectory(Path.Combine(RepositoryPath, backupJobId.ToString()));
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
                RepositoryPath,
                backupJobId.ToString(),
                storageId + _storageFileExtension);

            jobObjectsPaths.ForEach(jobObjectPath => SaveInArchive(storagePath, jobObjectPath));

            return storagePath;
        }

        public string CreateStorage(string jobObjectPath, Guid backupJobId, Guid storageId)
        {
            if (jobObjectPath == null)
                throw new ArgumentNullException(nameof(jobObjectPath));

            string storagePath = Path.Combine(
                RepositoryPath,
                backupJobId.ToString(),
                storageId + _storageFileExtension);

            SaveInArchive(storagePath, jobObjectPath);

            return storagePath;
        }

        public void DeleteStorages(List<string> storagesNames)
        {
            storagesNames.ForEach(File.Delete);
        }

        public void SaveInArchive(string storagePath, string jobObjectPath)
        {
            var archiveStream = new FileStream(storagePath, FileMode.OpenOrCreate);
            _compressor.Compress(archiveStream, jobObjectPath);
        }
    }
}