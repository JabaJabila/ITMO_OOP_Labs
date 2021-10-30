using System;
using System.Net.Sockets;

namespace BackupsClient.Entities
{
    public class Client : IDisposable
    {
        private TcpClient _client;
        public Client(string address, int port)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Port = port;
        }
        
        public string Address { get; }
        public int Port { get; }
        public NetworkStream NetworkStream { get; private set; }

        public void Connect()
        {
            _client = new TcpClient(Address, Port);
            NetworkStream = _client.GetStream();
        }

        public void Dispose()
        {
            NetworkStream.Close();
            _client.Close();
        }
    }
}