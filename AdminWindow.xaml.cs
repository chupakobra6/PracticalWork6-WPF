using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PracticalWork6
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private int port;
        private string username;
        private TcpServer _tcpServer;

        private List<Message> logMessages = new List<Message>();
        public AdminWindow(string username, int port)
        {
            InitializeComponent();
            this.username = username;
            this.port = port;

            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _tcpServer = new TcpServer(port, username);
            _tcpServer.ClientConnected += TcpServer_ClientConnected;
            _tcpServer.MessageReceived += TcpServer_MessageReceived;
            _tcpServer.ClientDisconnected += TcpServer_ClientDisconnected;

            LogMessage($"Server started at port: {port}.");

            await _tcpServer.StartAsync();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();
            string fullmessage = $"[{DateTime.Now}] {username}: {message}";

            if (!string.IsNullOrEmpty(message))
            {
                _tcpServer.Broadcast(fullmessage);
                MessageInput.Clear();
            }
        }

        private void UpdateClientsList()
        {
            List<string> clients = ConnectedClients.Get();
            UserList.ItemsSource = clients;
            UserList.Items.Refresh();
        }

        private void TcpServer_ClientConnected(object sender, string e)
        {
            string message = $"{e} присоединился к чату.";

            Application.Current.Dispatcher.Invoke(() =>
            {
                LogMessage($"[{e}] connected.");
                _tcpServer.Broadcast(message);
                UpdateClientsList();
            });
        }

        private void TcpServer_ClientDisconnected(object sender, string e)
        {
            string message = $"{e} покинул чат.";

            Application.Current.Dispatcher.Invoke(() =>
            {
                LogMessage($"[{e}] disconnected.");
                _tcpServer.Broadcast(message);
                UpdateClientsList();
            });
        }

        private void TcpServer_MessageReceived(object sender, Tuple<string, string> e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogMessage($"[{e.Item1}] sent: {e.Item2}");
                ChatLog.Items.Add(e.Item2);
            });
        }

        private void LogMessage(string message)
        {
            logMessages.Add(new Message(DateTime.Now, message));
        }

        private void LogsButton_Click(object sender, RoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow(logMessages);
            logWindow.ShowDialog();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _tcpServer.Stop();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void DiconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
