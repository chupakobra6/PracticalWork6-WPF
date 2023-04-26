using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        List<string> clientlist = new List<string>();

        public delegate void ClientConnectedEventHandler(object sender, string message);
        public event ClientConnectedEventHandler ClientConnected;

        public delegate void ClientDisconnectedEventHandler(object sender, string message);
        public event ClientDisconnectedEventHandler ClientDisconnected;

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
                _clientSockets.TryAdd(Guid.NewGuid().ToString(), clientSocket);

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

                    if (message.Contains("/disconnect"))
                    {
                        _clientSockets.TryRemove(_clientSockets.FirstOrDefault(x => x.Value == clientSocket).Key, out _);
                        clientSocket.Close();
                        string clientName = GetUserNameFromMessage(message);
                        clientlist.Remove(clientName);
                        ClientDisconnected?.Invoke(this, clientName);

                        await SendClientNameList();
                    }
                    else if (message.Contains("/newclient"))
                    {
                        string clientName = GetUserNameFromMessage(message);
                        clientlist.Add(clientName);
                        ClientConnected?.Invoke(this, clientName);
                        await SendClientNameList();
                    }
                    else
                    {
                        await Task.Factory.StartNew(() => Broadcast(message), TaskCreationOptions.LongRunning);
                    }
                }
                catch (SocketException)
                {
                    _cancellationTokenSource.Cancel();
                }
            }
        }

        private async Task<string> SendClientNameList()
        {
            string message = "/clientlist";
            foreach (string clientName in clientlist)
            {
                message += "." + clientName;
            }

            await Task.Factory.StartNew(() => Broadcast(message), TaskCreationOptions.LongRunning);
            return message;
        }

        private static string GetUserNameFromMessage(string message)
        {
            int clientNameStartPos = message.IndexOf(']') + 2;
            int clientNameEndPos = message.IndexOf(':', clientNameStartPos);
            string clientName = message.Substring(clientNameStartPos, clientNameEndPos - clientNameStartPos);
            return clientName;
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

        public delegate void UserListReceivedEventHandler(object sender, string[] userList);
        public event UserListReceivedEventHandler ClientListReceived;

        public delegate void DisconnectEventHandler(object sender);
        public event DisconnectEventHandler DisconnectEvent;

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
            await SendClientName();
            await Task.Delay(50);
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
                if (message.Contains("/clientlist"))
                {
                    string[] userList = message.Substring("/clientlist".Length + 1).Split('.');
                    ClientListReceived?.Invoke(this, userList);
                }
                else
                {
                    MessageReceived?.Invoke(this, message);
                }
            }
        }

        public async Task SendAsync(string message)
        {
            if (!GetConnection())
            {
                return;
            }

            string fullMessage = $"[{DateTime.Now}] {username}: {message}";

            byte[] buffer = Encoding.UTF8.GetBytes(fullMessage);
            await _clientSocket.SendAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

            if (message == "/disconnect")
            {
                Stop();
                DisconnectEvent?.Invoke(this);
            }
        }

        public async Task SendClientName()
        {
            await SendAsync($"/newclient");
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _clientSocket.Dispose();
        }

        public bool GetConnection()
        {
            return _clientSocket?.Connected ?? false;
        }
    }
}

