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

        List<Message> logMessages = new List<Message>(); // подумать
        public AdminWindow(string username, int port)
        {
            InitializeComponent();
            this.username = username;
            this.port = port;

            InitializeServer();
        }

        private async Task InitializeServer()
        {
            _tcpServer = new TcpServer(port, username);

            _tcpServer.StartAsync();

            LogMessage($"Server started at port: {port}.");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();

            if (!string.IsNullOrEmpty(message))
            {
                string fullmessage = $"[{DateTime.Now}] {username}: {message}";
                _tcpServer.Broadcast(fullmessage);
                ChatLog.Items.Add(fullmessage);
                LogMessage(fullmessage);
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
