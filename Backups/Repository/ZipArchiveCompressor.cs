using System.IO;
using System.IO.Compression;

namespace Backups.Repository
{
    public class ZipArchiveCompressor : ICompressor
    {
        private readonly CompressionLevel _compressionLevel;

        public ZipArchiveCompressor(CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            _compressionLevel = compressionLevel;
        }

        public void Compress(string storagePath, string jobObjectPath)
        {
            using var zipToOpen = new FileStream(storagePath, FileMode.OpenOrCreate);
            using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            archive.CreateEntryFromFile(
                jobObjectPath,
                Path.GetFileName(jobObjectPath),
                _compressionLevel);
        }
    }
}