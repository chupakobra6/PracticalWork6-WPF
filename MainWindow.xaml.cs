using System.Net;
using System.Windows;

namespace PracticalWork6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string ip = IPTextBox.Text;

            if (!Validation.IsValidUsername(username))
            {
                MessageBox.Show("Please enter a valid username (only letters, numbers and underscores are allowed).", "Invalid Username", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ConnectChatRadioButton.IsChecked == true)
            {
                if (!Validation.IsValidIP(ip))
                {
                    MessageBox.Show("Please enter a valid IP address.", "Invalid IP Address", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IPAddress adress = IPAddress.Parse(ip);

                UserWindow userWindow = new UserWindow(ip, 8888, username);
                userWindow.Show();
                Close();
            }
            else if (CreateChatRadioButton.IsChecked == true)
            {
                AdminWindow adminWindow = new AdminWindow(username, 8888);
                adminWindow.Show();
                Close();
            }
        }
    }
}
