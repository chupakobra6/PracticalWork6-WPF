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
        private readonly int port;
        private readonly string username;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static readonly ConcurrentDictionary<string, Socket> _clientSockets = new ConcurrentDictionary<string, Socket>();

        public TcpServer(int port, string username)
        {
            this.port = port;
            this.username = username;
        }

        public event EventHandler<string> ClientConnected;
        public event EventHandler<Tuple<string, string>> MessageReceived;
        public event EventHandler<string> ClientDisconnected;

        public async Task StartAsync()
        {
            using (var listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                _clientSockets.TryAdd(username, listeningSocket);
                OnClientConnected($"{username}");

                listeningSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                listeningSocket.Listen(100);

                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Socket clientSocket = await listeningSocket.AcceptAsync();

                    Task.Factory.StartNew(() => HandleClientAsync(clientSocket, _cancellationTokenSource.Token),
                        _cancellationTokenSource.Token,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default);
                }
            }
        }

        private async Task HandleClientAsync(Socket clientSocket, CancellationToken token)
        {
            OnClientConnected(username);

                while (!token.IsCancellationRequested)
                {
                    var buffer = new byte[1024];
                    int receivedbytes = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    string message = Encoding.UTF8.GetString(buffer, 0, receivedbytes);

                    await Broadcast(message);
                }
        }


        public async Task Broadcast(string message)
        {
            OnMessageReceived(new Tuple<string, string>(username, message));

            byte[] buffer = Encoding.UTF8.GetBytes(message);

            foreach (var clientSocket in _clientSockets.Values)
            {
                await clientSocket.SendAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            }
        }

        public static List<string> GetConnectedClients()
        {
            return _clientSockets.Keys.ToList();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        protected virtual void OnClientConnected(string clientName)
        {
            ConnectedClients.Add(clientName);
            ClientConnected?.Invoke(this, clientName);
        }

        protected virtual void OnMessageReceived(Tuple<string, string> messageData)
        {
            MessageReceived?.Invoke(this, messageData);
        }

        protected virtual void OnClientDisconnected(string clientName)
        {
            ConnectedClients.Remove(clientName);
            ClientDisconnected?.Invoke(this, clientName);
        }
    }

    public class TcpClient
    {
        private readonly string username;
        private readonly string _serverIp;
        private readonly int _serverPort;
        private readonly Socket _clientSocket;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private string _lastReceivedMessage;

        public event EventHandler<Tuple<string, string>> MessageReceived;

        public TcpClient(string serverIp, int serverPort, string username)
        {
            this.username = username;
            _serverIp = serverIp;
            _serverPort = serverPort;
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task ConnectAsync()
        {
            await _clientSocket.ConnectAsync(_serverIp, _serverPort);
        }

        public async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var buffer = new byte[1024];
                int receivedBytes = await _clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                if (receivedBytes == 0)
                {
                    break;
                }

                _lastReceivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                OnMessageReceived(new Tuple<string, string>(username, _lastReceivedMessage));
            }
        }

        protected virtual void OnMessageReceived(Tuple<string, string> messageData)
        {
            MessageReceived?.Invoke(this, messageData);
        }

        public async Task Send(string message)
        {
            if (message == "/disconnect")
            {
                Disconnect();
                return;
            }

            string fullMessage = $"[{DateTime.Now}] {username}: {message}";
            byte[] buffer = Encoding.UTF8.GetBytes(fullMessage);
            await _clientSocket.SendAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
        }

        public void Disconnect()
        {
            _cancellationTokenSource.Cancel();
            _clientSocket.Shutdown(SocketShutdown.Both);
            _cancellationTokenSource.Dispose();
        }
    }

    public class ConnectedClients
    {
        private static readonly List<string> _clients = new List<string>();

        public static void Add(string clientName)
        {
            lock (_clients)
            {
                _clients.Add(clientName);
            }
        }

        public static void Remove(string clientName)
        {
            lock (_clients)
            {
                _clients.Remove(clientName);
            }
        }

        public static List<string> Get()
        {
            lock (_clients)
            {
                return new List<string>(_clients);
            }
        }
    }

}

