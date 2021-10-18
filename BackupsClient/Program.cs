using System.Linq;
using Backups.Algorithms;
using Backups.Entities;
using Backups.Repository;
using BackupsClient.Entities;

namespace BackupsClient
{
    class Program
    {
        public static void Main()
        {
            var client = new Client("127.0.0.1", 8888);
            var repository = new ClientToServerRepository(
                new ZipArchiveCompressor(), 
                ".zip",
                client);

            var backupJob1 = new BackupJob(repository, new SingleStorageAlgorithm());
            var obj1 = new JobObject(@"d:\proj\a.txt");
            var obj2 = new JobObject(@"d:\proj\b.txt");
            
            backupJob1.AddJobObject(obj1);
            backupJob1.AddJobObject(obj2);
            backupJob1.CreateRestorePoint();
            backupJob1.DeleteJobObject(obj2);
            backupJob1.CreateRestorePoint();
            
            var backupJob2 = new BackupJob(repository, new SplitStoragesAlgorithm());
            backupJob2.AddJobObject(obj1);
            backupJob2.AddJobObject(obj2);
            backupJob2.CreateRestorePoint();
            backupJob2.DeleteJobObject(obj2);
            backupJob2.CreateRestorePoint();
            backupJob2.AddJobObject(obj2);
            backupJob2.CreateRestorePoint();
            
            backupJob2.DeleteRestorePoint(backupJob2.Backup.RestorePoints.Last());
        }
    }
}