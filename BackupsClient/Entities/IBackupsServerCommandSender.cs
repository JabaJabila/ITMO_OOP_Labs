using System.Net.Sockets;

namespace BackupsClient.Entities
{
    public interface IBackupsServerCommandSender
    {
        void SendFile(string path, string storageName, NetworkStream netStream);
        void CreateDirectory(string directoryName, NetworkStream netStream);
        void DeleteFile(string storageName, NetworkStream netStream);
    }
}