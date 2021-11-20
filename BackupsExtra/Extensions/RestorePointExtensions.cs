using System.Globalization;
using System.Linq;
using Backups.Entities;

namespace BackupsExtra.Extensions
{
    public static class RestorePointExtensions
    {
        public static string RestorePointInfo(this RestorePoint restorePoint)
        {
            string[] storagesInfo = restorePoint.Storages.Select(storage => "\t" + storage.StorageInfo()).ToArray();
            return $"restore point {restorePoint.Id} " +
                   $"created at: {restorePoint.CreationTime.ToString("g", CultureInfo.CurrentCulture)}" +
                   $"with {storagesInfo.Length} storages:\n" + string.Join('\n', storagesInfo);
        }
    }
}