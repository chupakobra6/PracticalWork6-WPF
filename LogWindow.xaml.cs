using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PracticalWork6
{
    /// <summary>
    /// Логика взаимодействия для LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        private List<Message> _logMessages;

        public LogWindow(List<Message> logMessages)
        {
            InitializeComponent();
            _logMessages = logMessages;

            foreach (Message message in _logMessages)
            {
                StackPanel_LogMessages.Children.Add(CreateMessageControl(message));
            }
        }

        private FrameworkElement CreateMessageControl(Message message)
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5, 5, 5, 0)
            };

            TextBlock timeStampTextBlock = new TextBlock
            {
                Text = message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss "),
            };
            stackPanel.Children.Add(timeStampTextBlock);

            TextBlock messageTextBlock = new TextBlock
            {
                Text = message.MessageText,
                TextWrapping = TextWrapping.Wrap,
            };
            stackPanel.Children.Add(messageTextBlock);

            return stackPanel;
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            StackPanel_LogMessages.Children.Clear();
            _logMessages.Clear();
        }
    }
}
