using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalWork6
{
    public class TcpServer
    {
        readonly int serverPort;
        Socket _serverSocket;
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        ConcurrentDictionary<string, Socket> _clientSockets = new ConcurrentDictionary<string, Socket>();

        public TcpServer(int serverPort)
        {
            this.serverPort = serverPort;
        }

        public async Task StartAsync()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
            _serverSocket.Listen(100);

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                Socket clientSocket = await _serverSocket.AcceptAsync();
                _clientSockets.TryAdd(clientSocket.LocalEndPoint.ToString(), clientSocket);

                Task.Factory.StartNew(() => HandleClientAsync(clientSocket), TaskCreationOptions.LongRunning);
            }
        }

        private async Task HandleClientAsync(Socket clientSocket)
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesReceived = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (bytesReceived == 0)
                    {
                        break;
                    }

                    byte[] messageBytes = new byte[bytesReceived];
                    Array.Copy(buffer, messageBytes, bytesReceived);
                    string message = Encoding.UTF8.GetString(messageBytes);

                    if (message == "/disconnect")
                    {
                        _cancellationTokenSource.Cancel();
                    }
                    else
                    {
                        await Task.Factory.StartNew(() => Broadcast(message),TaskCreationOptions.LongRunning);
                    }
                }
                catch (SocketException)
                {
                    _cancellationTokenSource.Cancel();
                }
            }

            _clientSockets.TryRemove(_clientSockets.FirstOrDefault(x => x.Value == clientSocket).Key, out _);
        }

        public async Task Broadcast(string message)
        {
            foreach (var clientSocket in _clientSockets.Values)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                await clientSocket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _serverSocket.Close();

            foreach (Socket socket in _clientSockets.Values)
            {
                socket.Close();
            }

            _clientSockets.Clear();
        }
    }

    public class TcpClient
    {
        readonly string serverIp;
        readonly int serverPort;
        readonly string username;
        Socket _clientSocket;
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public delegate void MessageReceivedEventHandler(object sender, string message);
        public event MessageReceivedEventHandler MessageReceived;

        public TcpClient(string serverIp, int serverPort, string username)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            this.username = username;
        }

        public async Task ConnectAsync()
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var endPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            await _clientSocket.ConnectAsync(endPoint);
        }

        public async Task ReceiveAsync()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var buffer = new byte[1024];
                var receivedBytes = await _clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                if (receivedBytes == 0)
                {
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                MessageReceived?.Invoke(this, message);
            }
        }

        public async Task SendAsync(string message)
        {
            string fullMessage = $"[{DateTime.Now}] {username}: {message}";

            byte[] buffer = Encoding.UTF8.GetBytes(fullMessage);
            await _clientSocket.SendAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
        }

        public async Task DisconnectAsync()
        {
            await SendAsync("/disconnect");
            _clientSocket.Close();
        }
    }
}

