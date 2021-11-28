using System;
using System.IO;
using System.Linq;
using Backups.Algorithms;
using Backups.Entities;
using Backups.Tools;
using BackupsExtra.Algorithms;
using BackupsExtra.Controllers;
using BackupsExtra.Loggers;
using BackupsExtra.Wrappers.BackupJob;
using BackupsExtra.Wrappers.Repositories;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    [TestFixture]
    public class BackupsExtraTest
    {
        private ExtendedLocalMemoryRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new ExtendedLocalMemoryRepository();
        }

        [TestCase("a.txt", "b.txt", "abcdef", "test\ntest")]
        [TestCase("document.docx", "table.csv", "doc test", "csv,test")]
        public void Create3PointsAndSetLimitBy1_2PointsDeleted(
            string fileName1,
            string fileName2,
            string file1,
            string file2)
        {
            string directory = Directory.GetCurrentDirectory();
            string path1 = Path.Combine(directory, fileName1);
            string path2 = Path.Combine(directory, fileName2);
            CreateTwoFiles(path1, path2, file1, file2);
            
            var backupJob = new ExtendedBackupJob(
                _repository,
                new SplitStoragesAlgorithm(),
                new EmptyLogger(),
                new ControllerByCount(3, new RestorePointDeleteAlgorithm()));

            var obj1 = new JobObject(path1);
            var obj2 = new JobObject(path2);

            backupJob.AddJobObject(obj1);
            backupJob.AddJobObject(obj2);

            backupJob.CreateRestorePoint();
            backupJob.CreateRestorePoint();
            backupJob.CreateRestorePoint();
            RestorePoint lastRestorePoint = backupJob.Backup.RestorePoints.Last();
            
            DeleteAfterFinish(path1, path2);
            
            backupJob.ChangeRestorePointController(new ControllerByCount(1, new RestorePointDeleteAlgorithm()));
            Assert.AreEqual(1, backupJob.Backup.RestorePoints.Count);
            Assert.AreEqual(lastRestorePoint.Id, backupJob.Backup.RestorePoints[0].Id);
        }
        
        [TestCase("a.txt", "b.txt", "abcdef", "test\ntest")]
        [TestCase("document.docx", "table.csv", "doc test", "csv,test")]
        public void Create3PointsAndSetLimitByDate_2PointsMerged(
            string fileName1,
            string fileName2,
            string file1,
            string file2)
        {
            string directory = Directory.GetCurrentDirectory();
            string path1 = Path.Combine(directory, fileName1);
            string path2 = Path.Combine(directory, fileName2);
            CreateTwoFiles(path1, path2, file1, file2);

            var backupJob = new ExtendedBackupJob(
                _repository,
                new SplitStoragesAlgorithm(),
                new EmptyLogger(),
                new ControllerByCount(3, new RestorePointMergeAlgorithm()));

            var obj1 = new JobObject(path1);
            var obj2 = new JobObject(path2);

            backupJob.AddJobObject(obj1);
            backupJob.AddJobObject(obj2);

            backupJob.CreateRestorePoint(DateTime.Today);
            backupJob.DeleteJobObject(obj2);
            backupJob.CreateRestorePoint(DateTime.Today.AddDays(1));
            backupJob.CreateRestorePoint(DateTime.Today.AddDays(2));

            backupJob.ChangeRestorePointController(
                new ControllerByDate(DateTime.Now.AddDays(1), new RestorePointMergeAlgorithm()));

            Assert.AreEqual(1, backupJob.Backup.RestorePoints.Count);
            Assert.AreEqual(2, backupJob.Backup.RestorePoints.First().Storages.Count);
            Assert.AreEqual(
                file2,
                string.Join('\n', _repository
                    .ReadFromStorage(backupJob.Backup.RestorePoints.First().Storages.Last().FullName)));
            
            DeleteAfterFinish(path1, path2);
        }

        [TestCase("a.txt", "b.txt", "abcdef", "test\ntest")]
        [TestCase("document.docx", "table.csv", "doc test", "csv,test")]
        public void SetLimitByHybridAndAllPointsShouldRemove_ThrowsException(
            string fileName1,
            string fileName2,
            string file1,
            string file2)
        {
            string directory = Directory.GetCurrentDirectory();
            string path1 = Path.Combine(directory, fileName1);
            string path2 = Path.Combine(directory, fileName2);
            CreateTwoFiles(path1, path2, file1, file2);

            var backupJob = new ExtendedBackupJob(
                _repository,
                new SplitStoragesAlgorithm(),
                new EmptyLogger(),
                new ControllerByCount(3, new RestorePointDeleteAlgorithm()));

            var obj1 = new JobObject(path1);
            var obj2 = new JobObject(path2);

            backupJob.AddJobObject(obj1);
            backupJob.AddJobObject(obj2);

            backupJob.CreateRestorePoint(DateTime.Today);
            backupJob.DeleteJobObject(obj2);
            backupJob.CreateRestorePoint(DateTime.Today.AddDays(1));
            backupJob.CreateRestorePoint(DateTime.Today.AddDays(2));

            backupJob.ChangeRestorePointController(
                new HybridController(2, DateTime.Now.AddDays(2), new RestorePointDeleteAlgorithm()));

            Assert.AreEqual(2, backupJob.Backup.RestorePoints.Count);
            Assert.Throws<BackupException>(() =>
            {
                backupJob.ChangeRestorePointController(
                    new HybridController(2, DateTime.Now.AddDays(2), new RestorePointDeleteAlgorithm(), false));
            });
            
            DeleteAfterFinish(path1, path2);
        }
        
        [TestCase("a.txt", "b.txt", "abcdef", "test\ntest")]
        [TestCase("document.docx", "table.csv", "doc test", "csv,test")]
        public void RestoreToOriginalLocation_SuccessfullyRestored(
            string fileName1,
            string fileName2,
            string file1,
            string file2)
        {
            string directory = Directory.GetCurrentDirectory();
            string path1 = Path.Combine(directory, fileName1);
            string path2 = Path.Combine(directory, fileName2);
            CreateTwoFiles(path1, path2, file1, file2);

            var backupJob = new ExtendedBackupJob(
                _repository,
                new SplitStoragesAlgorithm(),
                new EmptyLogger(),
                new ControllerByCount(3, new RestorePointDeleteAlgorithm()));

            var obj1 = new JobObject(path1);
            var obj2 = new JobObject(path2);

            backupJob.AddJobObject(obj1);
            backupJob.AddJobObject(obj2);
            backupJob.CreateRestorePoint();
            DeleteAfterFinish(path1, path2);
            backupJob.RestoreToOriginalLocation(backupJob.Backup.RestorePoints[0]);
            Assert.AreEqual(
                file1.Trim(), File.ReadAllText(path1).Trim());
            Assert.AreEqual(
                file2.Trim(), File.ReadAllText(path2).Trim());

            DeleteAfterFinish(path1, path2);
        }
        
        [TestCase("a.txt", "b.txt", "abcdef", "test\ntest")]
        [TestCase("document.docx", "table.csv", "doc test", "csv,test")]
        public void RestoreToTestDirectory_SuccessfullyRestored(
            string fileName1,
            string fileName2,
            string file1,
            string file2)
        {
            string directory = Directory.GetCurrentDirectory();
            string newDir = Path.Combine(directory, "TestDir");
            string path1 = Path.Combine(directory, fileName1);
            string path2 = Path.Combine(directory, fileName2);
            CreateTwoFiles(path1, path2, file1, file2);

            var backupJob = new ExtendedBackupJob(
                _repository,
                new SplitStoragesAlgorithm(),
                new EmptyLogger(),
                new ControllerByCount(3, new RestorePointDeleteAlgorithm()));

            var obj1 = new JobObject(path1);
            var obj2 = new JobObject(path2);

            backupJob.AddJobObject(obj1);
            backupJob.AddJobObject(obj2);
            backupJob.CreateRestorePoint();
            DeleteAfterFinish(path1, path2);
            backupJob.RestoreToDifferentLocation(backupJob.Backup.RestorePoints[0], newDir);
            path1 = Path.Combine(newDir, fileName1);
            path2 = Path.Combine(newDir, fileName2);
            Assert.AreEqual(
                file1.Trim(), File.ReadAllText(path1).Trim());
            Assert.AreEqual(
                file2.Trim(), File.ReadAllText(path2).Trim());

            DeleteAfterFinish(path1, path2);
            Directory.Delete(newDir);
        }
        
        private void CreateTwoFiles(
            string path1,
            string path2,
            string file1,
            string file2)
        {
            using (var fstream = new FileStream(path1, FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(file1);
                fstream.Write(array, 0, array.Length);
            }
            using (var fstream = new FileStream(path2, FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(file2);
                fstream.Write(array, 0, array.Length);
            }
        }

        private void DeleteAfterFinish(string path1, string path2)
        {
            File.Delete(path1);
            File.Delete(path2);
        }
    }
}