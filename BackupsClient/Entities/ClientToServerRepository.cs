using System;
using System.Collections.Generic;
using System.IO;
using Backups.Repository;

namespace BackupsClient.Entities
{
    public class ClientToServerRepository : IRepository
    {
        private readonly string _storageFileExtension;
        private readonly ICompressor _compressor;
        private readonly Client _client;
        private readonly IBackupsServerCommandSender _commandSender;
        
        public ClientToServerRepository(
            ICompressor compressor,
            string storageFileExtension,
            Client client,
            IBackupsServerCommandSender commandSender)
        {
            _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
            _storageFileExtension = storageFileExtension ??
                                    throw new ArgumentNullException(nameof(storageFileExtension));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _commandSender = commandSender ?? throw new ArgumentNullException(nameof(commandSender));
        }
        
        public void CreateBackupJobRepository(Guid backupJobId)
        {
            _client.Connect();
            _commandSender.CreateDirectory(backupJobId.ToString(), _client.NetworkStream);
        }

        public bool CheckIfJobObjectExists(string fullName)
        {
            return File.Exists(fullName);
        }

        public string CreateStorage(List<string> jobObjectsPaths, Guid backupJobId, Guid storageId)
        {
            if (jobObjectsPaths == null)
                throw new ArgumentNullException(nameof(jobObjectsPaths));

            string storageFullName = Path.Combine(
                backupJobId.ToString(),
                storageId + _storageFileExtension);
            
            string storagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                storageId + _storageFileExtension);

            _client.Connect();
            jobObjectsPaths.ForEach(jobObjectPath => _compressor.Compress(storagePath, jobObjectPath));
            _commandSender.SendFile(storagePath, storageFullName, _client.NetworkStream);
            File.Delete(storagePath);

            return storageFullName;
        }

        public string CreateStorage(string jobObjectPath, Guid backupJobId, Guid storageId)
        {
            if (jobObjectPath == null)
                throw new ArgumentNullException(nameof(jobObjectPath));
            
            string storageFullName = Path.Combine(
                backupJobId.ToString(),
                storageId + _storageFileExtension);
            
            string storagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                storageId + _storageFileExtension);
            
            _client.Connect();
            
            _compressor.Compress(storagePath, jobObjectPath);
            _commandSender.SendFile(storagePath, storageFullName, _client.NetworkStream);
            File.Delete(storagePath);
            
            return storageFullName;
        }

        public void DeleteStorages(List<string> storagesNames)
        {
            storagesNames.ForEach(storageName =>
            {
                _client.Connect();
                _commandSender.DeleteFile(storageName, _client.NetworkStream);
            }); 
        }
    }
}