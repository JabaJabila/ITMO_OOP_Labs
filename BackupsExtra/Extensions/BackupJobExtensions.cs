using Backups.Entities;

namespace BackupsExtra.Extensions
{
    public static class BackupJobExtensions
    {
        public static string BackupJobInfo(this BackupJob backupJob)
        {
            return $"backup job with id: {backupJob.Id.ToString()}";
        }
    }
}