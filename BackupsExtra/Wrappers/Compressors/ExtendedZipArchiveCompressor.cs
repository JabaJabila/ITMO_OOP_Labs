using System;
using System.IO;
using System.IO.Compression;
using Backups.Repository;

namespace BackupsExtra.Wrappers.Compressors
{
    public class ExtendedZipArchiveCompressor : IExtendedCompressor
    {
        private readonly ZipArchiveCompressor _compressor;

        public ExtendedZipArchiveCompressor(CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            _compressor = new ZipArchiveCompressor(compressionLevel);
        }

        public void Compress(Stream stream, string jobObjectPath)
        {
            _compressor.Compress(stream, jobObjectPath);
        }

        public bool IfStorageContainsOneObject(string pathToArchive)
        {
            using ZipArchive archive = ZipFile.Open(pathToArchive, ZipArchiveMode.Read);
            return archive.Entries.Count == 1;
        }

        public void Extract(string pathToArchive, string objectName, string location)
        {
            if (pathToArchive == null)
                throw new ArgumentNullException(nameof(pathToArchive));
            if (objectName == null)
                throw new ArgumentNullException(nameof(objectName));
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            using ZipArchive archive = ZipFile.Open(pathToArchive, ZipArchiveMode.Read);
            ZipArchiveEntry entry = archive.GetEntry(objectName);
            entry?.ExtractToFile(location, true);
        }
    }
}