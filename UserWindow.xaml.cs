using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PracticalWork6
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        string ip;
        int port;
        string username;
        TcpClient _tcpClient;

        public UserWindow(string ip, int port, string username)
        {
            InitializeComponent();
            this.ip = ip;
            this.port = port;
            this.username = username;


            InitializeClient();
        }

        private async Task InitializeClient()
        {
            _tcpClient = new TcpClient(ip, port, username);
            _tcpClient.MessageReceived += _tcpClient_MessageReceived;
            _tcpClient.ConnectAsync();
            _tcpClient.ReceiveAsync();
        }

        private void _tcpClient_MessageReceived(object sender, string message)
        {
            ChatLog.Items.Add(message);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();

            if (!string.IsNullOrEmpty(message))
            {
                string fullmessage = $"[{DateTime.Now}] {username}: {message}";
                _tcpClient.SendAsync(fullmessage);
                MessageInput.Clear();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void DiconnectButton_Click(object sender, RoutedEventArgs e)
        {
            _tcpClient.DisconnectAsync();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Close();
        }
    }
}
