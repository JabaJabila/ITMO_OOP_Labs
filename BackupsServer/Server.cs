using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BackupsServer.DataTypes;

namespace BackupsServer
{
    public sealed class Server : IDisposable
    {
        private readonly TcpListener _listener;
        private TcpClient _client;

        public Server(string ipAddress, int port, string directoryPath)
        {
            if (ipAddress == null)
                throw new ArgumentNullException(nameof(ipAddress));

            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _client = null;
            NetworkStream = null;

            DirectoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            Directory.CreateDirectory(DirectoryPath);
            _listener.Start();
        }

        public string DirectoryPath { get; }
        public NetworkStream NetworkStream { get; private set; }

        public void Work()
        {
            _client = _listener.AcceptTcpClient();
            NetworkStream = _client.GetStream();
            ChooseAction();
            NetworkStream.Close();
            _client.Close();
        }

        private void ChooseAction()
        {
            byte[] lengthData = new byte[sizeof(ActionOption)];
            NetworkStream.Read(lengthData, 0, sizeof(ActionOption));
            int option = BitConverter.ToInt32(lengthData, 0);
            if (!Enum.IsDefined(typeof(ActionOption), option)) return;
            switch ((ActionOption) option)
            {
                case ActionOption.ReadAndSaveFile:
                    ReadAndSaveFile();
                    break;
                case ActionOption.DeleteFile:
                    DeleteFile();
                    break;
                case ActionOption.CreateDirectory:
                    CreateJobDirectory();
                    break;
                case ActionOption.GetFile:
                    SendFile();
                    break;
                default:
                    return;
            }
        }

        private void SendFile()
        {
            string storagePath = GetLocation();
            string location = Path.Combine(DirectoryPath, storagePath);
            if (!File.Exists(location))
                return;

            string storageName = Path.GetFileName(location);
            
            byte[] lengthNameBytes = BitConverter.GetBytes(storageName.Length);
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(storageName);
            byte[] dataBytes = File.ReadAllBytes(location);
            byte[] dataLengthBytes = BitConverter.GetBytes(dataBytes.Length);

            NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
            NetworkStream.Write(fileNameBytes, 0, fileNameBytes.Length);
            NetworkStream.Write(dataLengthBytes, 0, dataLengthBytes.Length);
            NetworkStream.Write(dataBytes, 0, dataBytes.Length);
        }

        private string GetLocation()
        {
            long length = GetDataLength();
            string location = string.Empty;
            byte[] buffer = new byte[length];
            NetworkStream.Read(buffer, 0, buffer.Length);
            location += Encoding.ASCII.GetString(buffer, 0, buffer.Length);

            return location;
        }

        private void ReadAndSaveFile()
        {
            string location = GetLocation();
            int length = GetDataLength();
            byte[] buffer = new byte[length];

            NetworkStream.Read(buffer, 0, buffer.Length);

            File.WriteAllBytes(Path.Combine(DirectoryPath, location), buffer);
        }
        
        private int GetDataLength()
        {
            byte[] lengthData = new byte[sizeof(int)];
            NetworkStream.Read(lengthData, 0, sizeof(int));
            return BitConverter.ToInt32(lengthData, 0);
        }

        private void CreateJobDirectory()
        {
            string location = GetLocation();
            Directory.CreateDirectory(Path.Combine(DirectoryPath, location));
        }

        private void DeleteFile()
        {
            string location = GetLocation();
            File.Delete(Path.Combine(DirectoryPath, location));
        }

        public void Dispose()
        {
            _listener?.Stop();
            _client?.Dispose();
            NetworkStream?.Dispose();
        }
    }
}