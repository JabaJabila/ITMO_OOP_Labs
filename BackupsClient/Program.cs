using Backups.Algorithms;
using Backups.Entities;
using BackupsClient.Entities;
using BackupsExtra.Algorithms;
using BackupsExtra.Controllers;
using BackupsExtra.Loggers;
using BackupsExtra.Wrappers.BackupJob;
using BackupsExtra.Wrappers.Compressors;

namespace BackupsClient
{
    public class Program
    {
        public static void Main()
        {
            using var client = new Client("127.0.0.1", 8888);
            var repository = new ExtendedClientToServerRepository(
                new ExtendedZipArchiveCompressor(), 
                ".zip",
                client);

            var backupJob = new ExtendedBackupJob(
                repository,
                new SplitStoragesAlgorithm(),
                new ConsoleLogger(),
                new ControllerByCount(5, new RestorePointDeleteAlgorithm()));
            
            var obj1 = new JobObject(@"d:\proj\a.txt");
            var obj2 = new JobObject(@"D:\proj\b.txt");
            backupJob.AddJobObject(obj1);
            backupJob.AddJobObject(obj2);
            
            backupJob.CreateRestorePoint();
            backupJob.RestoreToDifferentLocation(backupJob.Backup.RestorePoints[0], @"d:\testRestore");
        }
    }
}