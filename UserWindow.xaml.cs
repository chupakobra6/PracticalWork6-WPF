using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PracticalWork6
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private TcpClient _tcpClient;
        private CancellationTokenSource _cancellationTokenSource;

        public UserWindow(string ip, int port, string username)
        {
            InitializeComponent();

            InitializeAsync(ip, port, username);
        }

        private async Task InitializeAsync(string ip, int port, string username)
        {
            _tcpClient = new TcpClient(ip, port, username);
            _tcpClient.MessageReceived += TcpClient_MessageReceived;
            await _tcpClient.ConnectAsync();

            _cancellationTokenSource = new CancellationTokenSource();
            await _tcpClient.ReceiveMessagesAsync(_cancellationTokenSource.Token);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                await _tcpClient.Send(message);
                MessageInput.Clear();
            }
        }

        private void UpdateClientsList()
        {
            List<string> clients = TcpServer.GetConnectedClients();
            UserList.ItemsSource = clients;
            UserList.Items.Refresh();
        }

        private void TcpClient_MessageReceived(object sender, Tuple<string, string> e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatLog.Items.Add($"{e.Item1}: {e.Item2}\n");
                UpdateClientsList();
            });
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _tcpClient.Disconnect();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void DiconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
