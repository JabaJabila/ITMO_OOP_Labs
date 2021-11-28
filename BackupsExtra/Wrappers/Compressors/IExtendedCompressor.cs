using Backups.Repository;

namespace BackupsExtra.Wrappers.Compressors
{
    public interface IExtendedCompressor : ICompressor
    {
        void Extract(string pathToArchive, string objectName, string location);
    }
}