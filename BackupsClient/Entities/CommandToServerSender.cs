using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using BackupsServer.DataTypes;

namespace BackupsClient.Entities
{
    public class CommandToServerSender : IBackupsServerCommandSender
    {
        public void SendFile(string path, string storageName, NetworkStream netStream)
        {
            byte[] serverOption = BitConverter.GetBytes((int)ActionOption.ReadAndSaveFile);
            byte[] lengthNameBytes = BitConverter.GetBytes(storageName.Length);
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(storageName);
            byte[] dataBytes = File.ReadAllBytes(path);
            byte[] dataLengthBytes = BitConverter.GetBytes(dataBytes.Length);

            netStream.Write(serverOption, 0, sizeof(int));
            netStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
            netStream.Write(fileNameBytes, 0, fileNameBytes.Length);
            netStream.Write(dataLengthBytes, 0, dataLengthBytes.Length);
            netStream.Write(dataBytes, 0, dataBytes.Length);
        }

        public void CreateDirectory(string directoryName, NetworkStream netStream)
        {
            byte[] serverOption = BitConverter.GetBytes((int)ActionOption.CreateDirectory);
            byte[] lengthNameBytes = BitConverter.GetBytes(directoryName.Length);
            byte[] directoryNameBytes = Encoding.ASCII.GetBytes(directoryName);
            
            netStream.Write(serverOption, 0, sizeof(int));
            netStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
            netStream.Write(directoryNameBytes, 0, directoryName.Length);
        }
        
        public void DeleteFile(string storageName, NetworkStream netStream)
        {
            byte[] serverOption = BitConverter.GetBytes((int)ActionOption.DeleteFile);
            byte[] lengthNameBytes = BitConverter.GetBytes(storageName.Length);
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(storageName);

            netStream.Write(serverOption, 0, sizeof(int));
            netStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
            netStream.Write(fileNameBytes, 0, fileNameBytes.Length);
        }
    }
}