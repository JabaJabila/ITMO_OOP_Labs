namespace Backups.Repository
{
    public interface ICompressor
    {
        void Compress(string storagePath, string jobObjectPath);
    }
}