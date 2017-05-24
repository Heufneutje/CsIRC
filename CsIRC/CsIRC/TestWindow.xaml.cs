﻿using System;
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
            _connection.Output.SendNICK("CSIRCTest");
            _connection.Output.SendUSER("Test", "Just Heufy messing around");
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
