using Backups.Repository;

namespace BackupsExtra.Wrappers.Compressors
{
    public interface IExtendedCompressor : ICompressor
    {
        bool IfStorageContainsOneObject(string pathToArchive);
        void Extract(string pathToArchive, string objectName, string location);
    }
}