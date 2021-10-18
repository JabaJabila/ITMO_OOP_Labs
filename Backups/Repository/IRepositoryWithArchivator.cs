namespace Backups.Repository
{
    public interface IRepositoryWithArchivator : IRepository
    {
        void SaveInArchive(string storagePath, string jobObjectPath);
    }
}