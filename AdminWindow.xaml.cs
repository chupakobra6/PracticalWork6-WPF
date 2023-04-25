using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace PracticalWork6
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        int port;
        string username;
        TcpServer _tcpServer;
        TcpClient _tcpClient;

        List<Message> logMessages = new List<Message>(); // подумать
        public AdminWindow(string username, int port)
        {
            InitializeComponent();
            this.username = username;
            this.port = port;

            InitializeServer();
            InitializeClient();
        }

        private async Task InitializeServer()
        {
            _tcpServer = new TcpServer(port);
            _tcpServer.StartAsync();

            LogMessage($"Server started at serverPort: {port}.");
        }

        private async Task InitializeClient()
        {
            _tcpClient = new TcpClient("127.0.0.1", port, username);
            await _tcpClient.ConnectAsync();
            _tcpClient.MessageReceived += TcpClient_MessageReceived; ;
            _tcpClient.ReceiveAsync();
        }

        private void TcpClient_MessageReceived(object sender, string message)
        {
            ChatLog.Items.Add(message);
            LogMessage(message);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();

            if (!string.IsNullOrEmpty(message))
            {
                await _tcpClient.SendAsync(message);
                MessageInput.Clear();
            }
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

        private void DiconnectButton_Click(object sender, RoutedEventArgs e)
        {
            _tcpServer.Stop();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Close();
        }
    }
}
