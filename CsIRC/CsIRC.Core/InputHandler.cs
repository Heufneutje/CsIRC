using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CsIRC.Core.Events;
using CsIRC.Utils;

namespace CsIRC.Core
{
    /// <summary>
    /// Handler that takes care of receiving messages and parsing them.
    /// </summary>
    public class InputHandler : IDisposable
    {
        private StreamReader _inputStream;
        private BackgroundWorker _inputWorker;
        private IRCConnection _connection;
        private bool _disposedValue; // IDisposable support. To detect redundant calls.

        /// <summary>
        /// Creates a new handler.
        /// </summary>
        /// <param name="inputStream">The input side of the connection's network stream.</param>
        /// <param name="connection">The connection this handler will be used for.</param>
        public InputHandler(StreamReader inputStream, IRCConnection connection)
        {
            _inputStream = inputStream;
            _connection = connection;
            _inputWorker = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            _inputWorker.DoWork += InputWorker_DoWork;
            _inputWorker.ProgressChanged += InputWorker_ProgressChanged;
            _inputWorker.RunWorkerAsync();
        }

        private void InputWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string inMessage;
            while (true)
            {
                try
                {
                    while ((inMessage = _inputStream.ReadLine()) != null)
                        _inputWorker.ReportProgress(0, inMessage);
                }
                catch (IOException)
                {
                    // TODO: Handle this properly.
                    break;
                }

                if (_inputWorker.CancellationPending)
                    break;
            }
        }

        private void InputWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            IRCMessage message = new IRCMessage((string)e.UserState);
            IRCMessageCancelEventArgs args = new IRCMessageCancelEventArgs(message);
            IRCEvents.OnMessageReceiving(this, args);
            if (args.Cancel)
                return;

            if (ushort.TryParse(message.Command, out ushort numeric))
                HandleNumeric(message);
            else
                HandleCommand(message);

            IRCEvents.OnMessageReceived(this, new IRCMessageEventArgs(message));
        }

        private void HandleCommand(IRCMessage message)
        {
            IRCHostmask prefix = new IRCHostmask(message.Prefix);
            IRCUser user = _connection.Users.FirstOrDefault(x => x.Nickname == prefix.Nickname);
            IRCChannel channel;
            string target;

            switch (message.Command)
            {
                case "JOIN":
                    if (user == null)
                    {
                        user = new IRCUser(prefix);
                        _connection.Users.Add(user);
                    }
                    target = message.Parameters.First();
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == target);
                    if (channel == null)
                    {
                        channel = new IRCChannel(target);
                        _connection.Channels.Add(channel);
                        _connection.Output.SendWHO(target);
                        _connection.Output.SendMODE(target);
                    }
                    if (!channel.Users.ContainsKey(user))
                        channel.Users.Add(user, new List<char>());
                    IRCEvents.OnChannelJoined(this, new ChannelUserCommandEventArgs(message, channel, user));
                    break;
                case "NICK":
                    string oldNick = user.Nickname;
                    string newNick = message.Parameters.First();
                    user.Nickname = newNick;
                    if (user.Nickname == _connection.CurrentNickname)
                        _connection.CurrentNickname = newNick;
                    IRCEvents.OnNicknameChanged(this, new NickChangedEventArgs(message, user, oldNick, newNick));
                    break;
                case "MODE":
                    List<string> modeParams = new List<string>();
                    target = message.Parameters.First();
                    if (message.Parameters.Count > 2)
                        modeParams = message.Parameters.Skip(2).ToList();
                    if (_connection.Support.ChannelTypes.Contains(target[0]))
                    {
                        channel = _connection.Channels.FirstOrDefault(x => x.Name == target);
                        if (channel != null)
                            channel.SetModes(new ModeString(message.Parameters[1], modeParams, _connection.Support.ChannelModes, _connection.Support.StatusModes.Keys.ToList()));
                    }
                    else if (target == _connection.CurrentNickname)
                        _connection.SetUserModes(new ModeString(message.Parameters[1], modeParams, _connection.Support.UserModes, null));
                    break;
                case "NOTICE":
                case "PRIVMSG":
                    if (user == null)
                        user = new IRCUser(prefix);
                    target = message.Parameters.First();
                    if (_connection.Support.ChannelTypes.Contains(target[0]))
                    {
                        channel = _connection.Channels.FirstOrDefault(x => x.Name == target);
                        if (channel == null)
                            channel = new IRCChannel(target);

                        IRCEvents.OnMessageCommandReceived(this, new MessageCommandEventArgs(message, channel, user));
                    }
                    else
                        IRCEvents.OnMessageCommandReceived(this, new MessageCommandEventArgs(message, user));
                    break;
                case "PART":
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == message.Parameters.First());
                    if (channel == null)
                        return;

                    bool fireEvent = false;
                    if (_connection.CurrentNickname == user.Nickname)
                    {
                        _connection.Channels.Remove(channel);
                        fireEvent = true;
                    }
                    else if (channel.Users.ContainsKey(user))
                    {
                        _connection.Users.Remove(user);
                        fireEvent = true;
                    }

                    if (fireEvent)
                    {
                        string reason = null;
                        if (message.Parameters.Count > 1)
                            reason = message.Parameters[1];
                        IRCEvents.OnChannelParted(this, new ChannelUserReasonCommandEventArgs(message, channel, user, reason));
                    }
                    break;
                case "PING":
                    _connection.Output.SendPONG(message.Parameters.First());
                    break;
                case "TOPIC":
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == message.Parameters.First());
                    if (channel == null)
                        return;

                    string oldTopic = channel.Topic;
                    string newTopic = message.Parameters[1];
                    channel.Topic = newTopic;
                    channel.TopicSetDate = DateTime.Now;
                    channel.TopicSetter = user.GetHostmask();
                    IRCEvents.OnTopicChanged(this, new TopicChangedEventArgs(message, channel, user, oldTopic, newTopic));
                    break;
            }
        }

        private void HandleNumeric(IRCMessage message)
        {
            IRCChannel channel;
            switch (message.Command)
            {
                case "001": // RPL_WELCOME
                    _connection.IsLoggedIn = true;
                    break;
                case "004": // RPL_MYINFO
                    _connection.Support.ServerName = message.Parameters[1];
                    _connection.Support.ServerVersion = message.Parameters[2];
                    _connection.Support.UserModes.Clear();
                    foreach (char modeChar in message.Parameters[3])
                    {
                        // If it's not mode s, we should assume the mode has no parameter unless ISUPPORT says otherwise.
                        ModeType modeType = modeChar == 's' ? ModeType.ParamSet : ModeType.NoParam;
                        _connection.Support.UserModes.Add(modeChar, modeType);
                    }
                    break;
                case "005": // RPL_ISUPPORT
                    _connection.Support.ParseTokens(message.Parameters);
                    break;
                case "324":
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == message.Parameters[1]);
                    List<string> modeParams = new List<string>();
                    if (message.Parameters.Count > 3)
                        modeParams = message.Parameters[3].Split(' ').ToList();
                    channel.SetModes(new ModeString(message.Parameters[2], modeParams, _connection.Support.ChannelModes, _connection.Support.StatusModes.Keys.ToList()));
                    break;
                case "329": // RPL_CREATIONTIME
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == message.Parameters[1]);
                    if (channel != null)
                        channel.CreationTime = DateTimeUtils.FromUnixTime(Convert.ToInt64(message.Parameters[2]));
                    break;
                case "332": // RPL_TOPIC
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == message.Parameters[1]);
                    if (channel != null)
                        channel.Topic = message.Parameters[2];
                    break;
                case "333": // RPL_TOPICWHOTIME
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == message.Parameters[1]);
                    if (channel != null)
                    {
                        channel.TopicSetter = new IRCHostmask(message.Parameters[2]);
                        channel.TopicSetDate = DateTimeUtils.FromUnixTime(Convert.ToInt64(message.Parameters[3]));
                    }
                    break;
                case "352": // RPL_WHOREPLY
                    IRCUser whoUser = _connection.Users.FirstOrDefault(x => x.Nickname == message.Parameters[5]);
                    if (whoUser == null)
                        return;

                    whoUser.Username = message.Parameters[2];
                    whoUser.Hostname = message.Parameters[3];
                    whoUser.Server = message.Parameters[4];
                    List<char> flags = message.Parameters[6].ToCharArray().ToList();
                    if (flags.Pop(0) == 'G')
                        whoUser.IsAway = true;
                    if (flags.Any() && flags.First() == '*')
                    {
                        whoUser.IsOper = true;
                        flags.RemoveAt(0);
                    }
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == message.Parameters[1]);
                    if (channel != null && channel.Users.ContainsKey(whoUser))
                    {
                        channel.Users[whoUser] = new List<char>();
                        foreach (char symbolChar in flags)
                            channel.Users[whoUser].Add(_connection.Support.StatusSymbols[symbolChar]);
                    }
                    string[] hopsGecos = message.Parameters[7].Split(' ');
                    whoUser.Hops = Convert.ToUInt32(hopsGecos[0]);
                    if (hopsGecos.Length > 1)
                        whoUser.Gecos = string.Join(" ", hopsGecos.Skip(1));
                    IRCEvents.OnUserlistUpdated(this, new UserlistUpdatedEventArgs(message, channel, new List<IRCUser> { whoUser }));
                    break;
                case "353": // RPL_NAMREPLY
                    List<IRCUser> updatedUsers = new List<IRCUser>();
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == message.Parameters[2]);
                    if (channel.UserlistComplete)
                    {
                        channel.UserlistComplete = false;
                        channel.Users.Clear();
                    }
                    foreach (IRCHostmask prefix in message.Parameters[3].Split(' ').Select(x => new IRCHostmask(x)))
                    {
                        List<char> ranks = new List<char>();
                        while (_connection.Support.StatusSymbols.ContainsKey(prefix.Nickname[0]))
                        {
                            ranks.Add(_connection.Support.StatusSymbols[prefix.Nickname[0]]);
                            prefix.Nickname = prefix.Nickname.Substring(1);
                        }
                        IRCUser user = _connection.Users.FirstOrDefault(x => x.Nickname == prefix.Nickname);
                        if (user == null)
                        {
                            user = new IRCUser(prefix);
                            _connection.Users.Add(user);
                            updatedUsers.Add(user);
                        }
                        channel.Users.Add(user, ranks);
                    }
                    IRCEvents.OnUserlistUpdated(this, new UserlistUpdatedEventArgs(message, channel, updatedUsers));
                    break;
                case "366": // RPL_ENDOFNAMES
                    channel = _connection.Channels.FirstOrDefault(x => x.Name == message.Parameters[1]);
                    if (channel != null)
                        channel.UserlistComplete = true;
                    break;
            }
        }

        #region IDisposable Support

        /// <summary>
        /// Disposable pattern implementation.
        /// </summary>
        /// <param name="disposing">Should dispose Y/N?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _inputWorker.CancelAsync();
                    _inputStream.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Disposable pattern implementation.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}
