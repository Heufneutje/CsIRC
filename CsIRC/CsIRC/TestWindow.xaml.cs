using System;
using System.Windows;
using CsIRC.Core;
using CsIRC.Core.Events;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Controls;
using System.Collections.Generic;

namespace CsIRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private IRCConnection _connection;
        private Dictionary<string, MessageControl> _messageControls;

        public TestWindow()
        {
            InitializeComponent();
            _messageControls = new Dictionary<string, MessageControl>();
            _connection = new IRCConnection();
            _connection.Connect("heufneutje.net", 6670);
            IRCEvents.MessageReceived += IRCEvents_MessageReceived;
            IRCEvents.MessageSent += IRCEvents_MessageReceived;
            IRCEvents.MessageCommandReceived += IRCEvents_MessageCommandReceived;
            IRCEvents.UserlistUpdated += IRCEvents_UserlistUpdated;
            IRCEvents.ChannelJoined += IRCEvents_ChannelJoined;
            IRCEvents.TopicChanged += IRCEvents_TopicChanged;
            _connection.CurrentNickname = "CSIRCTest";
            _connection.Capability.RequestAvailableCapabilities();
            _connection.Output.SendNICK("CSIRCTest");
            _connection.Output.SendUSER("Test", "Just Heufy messing around");

            _dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.MetroTheme();
            _dockingManager.ShowSystemMenu = true;
        }

        private void IRCEvents_TopicChanged(object sender, TopicChangedEventArgs args)
        {
            if (_messageControls.ContainsKey(args.Channel.Name))
                _messageControls[args.Channel.Name].SetTopic(args.NewTopic);
        }

        private void IRCEvents_ChannelJoined(object sender, ChannelUserCommandEventArgs args)
        {
            if (args.User.Nickname == _connection.CurrentNickname)
            {
                MessageControl newChannelControl = new MessageControl(_connection, args.Channel);
                _messageControls.Add(args.Channel.Name, newChannelControl);

                LayoutAnchorable la = new LayoutAnchorable { Title = args.Channel.Name, FloatingHeight = 400, FloatingWidth = 500, Content = newChannelControl };
                la.AddToLayout(_dockingManager, AnchorableShowStrategy.Left);
                la.DockAsDocument();
            }
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
            if (args.Channel != null && _messageControls.ContainsKey(args.Channel.Name))
                _messageControls[args.Channel.Name].AppendMessage($"<{args.User.Nickname}> {args.MessageBody}{Environment.NewLine}");
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
