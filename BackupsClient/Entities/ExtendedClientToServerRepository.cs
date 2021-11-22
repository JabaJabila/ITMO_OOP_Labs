using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Backups.Repository;
using Backups.Tools;
using BackupsExtra.Wrappers.Compressors;
using BackupsExtra.Wrappers.Repositories;
using BackupsServer.DataTypes;

namespace BackupsClient.Entities
{
    public class ExtendedClientToServerRepository : IExtendedRepository, IRepositoryWithArchivator
    {
        private readonly ClientToServerRepository _repository;
        private readonly Dictionary<string, List<string>> _objectsOriginalLocation;
        private readonly Client _client;
        private readonly IExtendedCompressor _compressor;

        public ExtendedClientToServerRepository(
            IExtendedCompressor compressor,
            string storageFileExtension,
            Client client)
        {
            _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _repository = new ClientToServerRepository(compressor, storageFileExtension, client);
            _objectsOriginalLocation = new Dictionary<string, List<string>>();
        }

        public void CreateBackupJobRepository(Guid backupJobId)
            => _repository.CreateBackupJobRepository(backupJobId);

        public bool CheckIfJobObjectExists(string fullName)
            => _repository.CheckIfJobObjectExists(fullName);

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
            => _repository.SaveInArchive(storagePath, jobObjectPath);

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

            foreach (string storagePath in storagePaths)
            {
                List<string> objectPaths = _objectsOriginalLocation[storagePath];
                _client.Connect();
                GetStorageFromServer(storagePath);
                foreach (string objectPath in objectPaths)
                {
                    string filename = Path.GetFileName(objectPath);
                    if (File.Exists(Path.Combine(pathToRestore, Path.GetFileName(objectPath))))
                    {
                        throw new BackupException($"Impossible to restore to {pathToRestore}" +
                                                  $"file {filename} already exists");
                    }
                    
                    _compressor.Extract(
                        Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(storagePath)), 
                        filename, 
                        Path.Combine(pathToRestore, filename));
                }
            }
        }

        public void RestoreToOriginalLocation(IReadOnlyCollection<string> storagePaths)
        {
            if (storagePaths == null)
                throw new ArgumentNullException(nameof(storagePaths));

            storagePaths.ToList().ForEach(storagePath =>
            {
                _client.Connect();
                GetStorageFromServer(storagePath);
                List<string> objectPaths = _objectsOriginalLocation[storagePath];
                objectPaths.ForEach(objectPath =>
                    _compressor.Extract(
                        Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(storagePath)),
                        Path.GetFileName(objectPath),
                        objectPath));
            });
        }

        private void GetStorageFromServer(string storagePath)
        {
            byte[] serverOption = BitConverter.GetBytes((int)ActionOption.GetFile);
            byte[] lengthNameBytes = BitConverter.GetBytes(storagePath.Length);
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(storagePath);
            
            _client.NetworkStream.Write(serverOption, 0, sizeof(int));
            _client.NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
            _client.NetworkStream.Write(fileNameBytes, 0, fileNameBytes.Length);

            byte[] lengthOfFileNameData = new byte[sizeof(int)];
            _client.NetworkStream.Read(lengthOfFileNameData, 0, sizeof(int));
            int lengthOfFileName = BitConverter.ToInt32(lengthOfFileNameData, 0);
            
            if (lengthOfFileName == 0)
                throw new BackupException($"Storage {storagePath} wasn't found on server!");

            byte[] fileNameData = new byte[lengthOfFileName];
            _client.NetworkStream.Read(fileNameData, 0, lengthOfFileName);
            string fileName = Encoding.ASCII.GetString(fileNameData, 0, lengthOfFileName);
            
            byte[] lengthData = new byte[sizeof(int)];
            _client.NetworkStream.Read(lengthData, 0, sizeof(int));
            int lengthOfFile = BitConverter.ToInt32(lengthData, 0);

            byte[] buffer = new byte[lengthOfFile];
            _client.NetworkStream.Read(buffer, 0, buffer.Length);
            
            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), fileName), buffer);
        }
    }
}