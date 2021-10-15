using Backups.Algorithms;
using Backups.Entities;
using Backups.Repository;

namespace Backups
{
    internal class Program
    {
        private static void Main()
        {
            var repo = new LocalFilesRepository(@"d:\backupjob", new ZipArchiveCompressor(), ".zip");
            var joba = new BackupJob(repo, new SingleStorageAlgorithm());
            var obj1 = new JobObject(@"d:\proj\a.txt");
            var obj2 = new JobObject(@"d:\proj\b.txt");

            joba.AddJobObject(obj1);
            joba.AddJobObject(obj2);
            joba.CreateRestorePoint();
        }
    }
}
