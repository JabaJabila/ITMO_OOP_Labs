using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Backups.Repository;
using BackupsServer.DataTypes;
using Newtonsoft.Json;

namespace BackupsClient.Entities
{
    public class ClientToServerRepository : IRepositoryWithArchivator
    {
        [JsonProperty("storageFileExtension")]
        private readonly string _storageFileExtension;
        [JsonProperty("compressor")]
        private readonly ICompressor _compressor;
        [JsonProperty("client")]
        private readonly Client _client;

        public ClientToServerRepository(
            ICompressor compressor,
            string storageFileExtension,
            Client client)
        {
            _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
            _storageFileExtension = storageFileExtension ??
                                    throw new ArgumentNullException(nameof(storageFileExtension));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        
        public void CreateBackupJobRepository(Guid backupJobId)
        {
            _client.Connect();
            CreateDirectory(backupJobId.ToString());
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
            jobObjectsPaths.ForEach(jobObjectPath => SaveInArchive(storagePath, jobObjectPath));
            SendFile(storagePath, storageFullName);
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
            SaveInArchive(storagePath, jobObjectPath);
            SendFile(storagePath, storageFullName);
            File.Delete(storagePath);
            
            return storageFullName;
        }

        public void DeleteStorages(List<string> storagesNames)
        {
            storagesNames.ForEach(storageName =>
            {
                _client.Connect();
                DeleteFile(storageName);
            }); 
        }

        public void SaveInArchive(string storagePath, string jobObjectPath)
        {
            var archiveStream = new FileStream(storagePath, FileMode.OpenOrCreate);
            _compressor.Compress(archiveStream, jobObjectPath);
        }

        private void SendFile(string path, string storageName)
        {
            byte[] serverOption = BitConverter.GetBytes((int)ActionOption.ReadAndSaveFile);
            byte[] lengthNameBytes = BitConverter.GetBytes(storageName.Length);
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(storageName);
            byte[] dataBytes = File.ReadAllBytes(path);
            byte[] dataLengthBytes = BitConverter.GetBytes(dataBytes.Length);

            _client.NetworkStream.Write(serverOption, 0, sizeof(int));
            _client.NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
            _client.NetworkStream.Write(fileNameBytes, 0, fileNameBytes.Length);
            _client.NetworkStream.Write(dataLengthBytes, 0, dataLengthBytes.Length);
            _client.NetworkStream.Write(dataBytes, 0, dataBytes.Length);
        }

        private void CreateDirectory(string directoryName)
        {
            byte[] serverOption = BitConverter.GetBytes((int)ActionOption.CreateDirectory);
            byte[] lengthNameBytes = BitConverter.GetBytes(directoryName.Length);
            byte[] directoryNameBytes = Encoding.ASCII.GetBytes(directoryName);
            
            _client.NetworkStream.Write(serverOption, 0, sizeof(int));
            _client.NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
            _client.NetworkStream.Write(directoryNameBytes, 0, directoryName.Length);
        }

        private void DeleteFile(string storageName)
        {
            byte[] serverOption = BitConverter.GetBytes((int)ActionOption.DeleteFile);
            byte[] lengthNameBytes = BitConverter.GetBytes(storageName.Length);
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(storageName);

            _client.NetworkStream.Write(serverOption, 0, sizeof(int));
            _client.NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
            _client.NetworkStream.Write(fileNameBytes, 0, fileNameBytes.Length);
        }
    }
}