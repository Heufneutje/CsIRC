using CsIRC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CsIRC
{
    /// <summary>
    /// Interaction logic for MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {
        private IRCConnection _connection;
        private IRCChannel _channel;
        private IRCUser _user;

        public MessageControl(IRCConnection connection, IRCChannel channel)
        {
            InitializeComponent();
            _connection = connection;
            _channel = channel;
        }

        public MessageControl(IRCConnection connection, IRCUser user)
        {
            InitializeComponent();
            _connection = connection;
            _user = user;
        }

        public void AppendMessage(string message)
        {
            _conversationTextBox.Text = $"{_conversationTextBox.Text}{message}{Environment.NewLine}";
        }

        public void SetTopic(string topic)
        {
            _topicTextBox.Text = topic;
        }

        private void _messageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (_messageTextBox.Text.StartsWith("/"))
                _connection.Output.SendRaw(_messageTextBox.Text.Substring(1));
            else if (_channel != null)
                _connection.Output.SendPRIVMSG(_channel, _messageTextBox.Text);
            else if (_user != null)
                _connection.Output.SendPRIVMSG(_user, _messageTextBox.Text);

            _messageTextBox.Text = string.Empty;
        }
    }
}
