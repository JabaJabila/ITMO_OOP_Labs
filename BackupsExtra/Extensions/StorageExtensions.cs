using Backups.Entities;

namespace BackupsExtra.Extensions
{
    public static class StorageExtensions
    {
        public static string StorageInfo(this Storage storage)
        {
            return $"storage at: {storage.FullName}";
        }
    }
}