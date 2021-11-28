using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace Backups.Repository
{
    public class ZipArchiveCompressor : ICompressor
    {
        [JsonProperty("compressionLevel")]
        private readonly CompressionLevel _compressionLevel;

        public ZipArchiveCompressor(CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            _compressionLevel = compressionLevel;
        }

        public void Compress(Stream stream, string jobObjectPath)
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Update);
            archive.CreateEntryFromFile(
                jobObjectPath,
                Path.GetFileName(jobObjectPath),
                _compressionLevel);
        }
    }
}