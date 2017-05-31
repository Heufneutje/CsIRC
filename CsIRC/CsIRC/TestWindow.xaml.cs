using System;
using System.Windows;
using CsIRC.Core;
using CsIRC.Core.Events;

namespace CsIRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private IRCConnection _connection;

        public TestWindow()
        {
            InitializeComponent();
            _connection = new IRCConnection();
            _connection.Connect("heufneutje.net", 6670);
            IRCEvents.MessageReceived += IRCEvents_MessageReceived;
            IRCEvents.MessageSent += IRCEvents_MessageReceived;
            IRCEvents.MessageCommandReceived += IRCEvents_MessageCommandReceived;
            IRCEvents.UserlistUpdated += IRCEvents_UserlistUpdated;
            _connection.CurrentNickname = "CSIRCTest";
            _connection.Output.SendNICK("CSIRCTest");
            _connection.Output.SendUSER("Test", "Just Heufy messing around");

            _dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.MetroTheme();
            _dockingManager.ShowSystemMenu = true;
        }

        private void IRCEvents_UserlistUpdated(object sender, UserlistUpdatedEventArgs args)
        {
            //foreach (IRCUser user in args.UpdatedUsers)
            //{
            //    string status = args.Channel.Users[user];
            //    if (status.Length > 0)
            //        listBox.Items.Add($"{_connection.Support.StatusModes[status[0]]}{user.Nickname}");
            //    else
            //        listBox.Items.Add(user.Nickname);
            //}
        }

        private void IRCEvents_MessageCommandReceived(object sender, MessageCommandEventArgs args)
        {
            textBox.Text += $"<{args.User.Nickname}> {args.MessageBody}{Environment.NewLine}";
        }

        private void IRCEvents_MessageReceived(object sender, IRCMessageEventArgs args)
        {
            textBox.Text += $"{args.Message.RawLine}{Environment.NewLine}";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _connection.Disconnect();
        }
    }
}
