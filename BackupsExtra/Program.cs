using System;
using System.IO;
using Backups.Algorithms;
using Backups.Entities;
using BackupsExtra.Algorithms;
using BackupsExtra.Controllers;
using BackupsExtra.Loggers;
using BackupsExtra.Wrappers.BackupJob;
using BackupsExtra.Wrappers.Compressors;
using BackupsExtra.Wrappers.Repositories;

namespace BackupsExtra
{
    internal class Program
    {
        private static void Main()
        {
            var repo = new ExtendedLocalFilesRepository(@"d:\backupjob", new ExtendedZipArchiveCompressor(), ".zip");
            var job = new ExtendedBackupJob(
                repo,
                new SplitStoragesAlgorithm(),
                new FileLogger(@"d:\backupjob\backup_log.txt", true),
                new ControllerByCount(4, new RestorePointDeleteAlgorithm()));

            var obj1 = new JobObject(@"d:\proj\a.txt");
            var obj2 = new JobObject(@"d:\proj\b.txt");

            job.AddJobObject(obj1);
            job.AddJobObject(obj2);
            job.CreateRestorePoint(DateTime.Today);
            job.DeleteJobObject(obj1);
            job.CreateRestorePoint(DateTime.Today.AddDays(1));
            job.CreateRestorePoint(DateTime.Today.AddDays(2));
            job.CreateRestorePoint(DateTime.Today.AddDays(3));

            using var streamWrite = new FileStream(@"d:\backupjob\joba.data", FileMode.Create);
            job.SaveBackupJob(streamWrite);
            streamWrite.Dispose();
            using var streamRead = new FileStream(@"d:\backupjob\joba.data", FileMode.Open);
            job = ExtendedBackupJob.LoadBackupJob(streamRead);

            job.ChangeRestorePointController(
                new HybridController(1, DateTime.Now.AddDays(2), new RestorePointMergeAlgorithm()));

            using StreamWriter sw = File.AppendText(@"d:\proj\b.txt");
            sw.WriteLine("dd");
            sw.Dispose();
            job.RestoreToOriginalLocation(job.Backup.RestorePoints[0]);
        }
    }
}
