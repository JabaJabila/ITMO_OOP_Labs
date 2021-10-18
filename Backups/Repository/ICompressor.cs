using System.IO;

namespace Backups.Repository
{
    public interface ICompressor
    {
        void Compress(Stream stream, string jobObjectPath);
    }
}