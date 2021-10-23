using System.IO;
using Backups.Entities;
using Backups.Algorithms;
using Backups.Repository;
using NUnit.Framework;

namespace Backups.Tests
{
    [TestFixture]
    public class Tests
    {
        private BackupJob _backupJob;
        private LocalMemoryRepository _repository;
        [SetUp]
        public void Setup()
        {
            _repository = new LocalMemoryRepository();
            _backupJob = new BackupJob(_repository, new SplitStoragesAlgorithm());
        }

        [TestCase("a.txt", "b.txt", "abcdef", "test\ntest")]
        [TestCase("document.docx", "table.csv", "doc test", "csv,test")]
        public void CreateRestorePointSplitStorages_SuccessfullyStored(
            string fileName1,
            string fileName2,
            string file1,
            string file2)
        {
            string directory = Directory.GetCurrentDirectory();
            string path1 = Path.Combine(directory, fileName1);
            string path2 = Path.Combine(directory, fileName2);
            
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

            var obj1 = new JobObject(path1);
            var obj2 = new JobObject(path2);

            _backupJob.AddJobObject(obj1);
            _backupJob.AddJobObject(obj2);
            _backupJob.CreateRestorePoint();
            string out1 = string.Join(
                "",
                _repository.ReadFromStorage(
                    _backupJob.Backup.RestorePoints[0].Storages[0].FullName));
            string out2 = string.Join(
                "",
                _repository.ReadFromStorage(
                    _backupJob.Backup.RestorePoints[0].Storages[1].FullName));
            
            File.Delete(path1);
            File.Delete(path2);
            
            Assert.AreEqual(file1, out1);
            Assert.AreEqual(file2, out2);
        }

        [TestCase("a.txt", "b.txt", "abcdef", "test\ntest")]
        [TestCase("document.docx", "table.csv", "doc test", "csv,test")]
        public void AddJobObjectAndDelete_PointsAndStoragesCreated(
            string fileName,
            string fileNameToDelete,
            string file,
            string fileToDelete)
        {
            const int expectedStorages = 3;
            const int expectedRestorePoints = 2;
            
            string directory = Directory.GetCurrentDirectory();
            string path1 = Path.Combine(directory, fileName);
            string path2 = Path.Combine(directory, fileNameToDelete);
    
            using (var fstream = new FileStream(path1, FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(file);
                fstream.Write(array, 0, array.Length);
            }
            using (var fstream = new FileStream(path2, FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(fileToDelete);
                fstream.Write(array, 0, array.Length);
            }

            var obj1 = new JobObject(path1);
            var obj2 = new JobObject(path2);

            _backupJob.AddJobObject(obj1);
            _backupJob.AddJobObject(obj2);
            _backupJob.CreateRestorePoint();
            _backupJob.DeleteJobObject(obj2);
            _backupJob.CreateRestorePoint();
            
            File.Delete(path1);
            File.Delete(path2);

            Assert.AreEqual(expectedRestorePoints,_backupJob.Backup.RestorePoints.Count);
            Assert.AreEqual(
                expectedStorages,
                _backupJob.Backup.RestorePoints[0].Storages.Count +
                _backupJob.Backup.RestorePoints[1].Storages.Count);
        }
    }
}