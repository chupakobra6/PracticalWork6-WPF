using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

                // Открываем окно пользователя чата
                UserWindow userWindow = new UserWindow(ip, 8888, username);
                userWindow.Show();
                Close();
            }
            else if (CreateChatRadioButton.IsChecked == true)
            {
                // Открываем окно администратора чата
                AdminWindow adminWindow = new AdminWindow(username, 8888); // Создаём окно админа и передаём в него порт
                adminWindow.Show();
                Close();
            }
        }
    }
}
