using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsIRC.Core.Events;

namespace CsIRC.Core
{
    /// <summary>
    /// Handler that takes care of sending messages to the server.
    /// </summary>
    public class OutputHandler : IDisposable
    {
        private IRCConnection _connection;
        private bool _disposedValue; // IDisposable support. To detect redundant calls.
        private StreamWriter _outputStream;

        /// <summary>
        /// Creates a new handler.
        /// </summary>
        /// <param name="outputStream">The output side of the connection's network stream.</param>
        public OutputHandler(StreamWriter outputStream, IRCConnection connection)
        {
            _outputStream = outputStream;
            _outputStream.AutoFlush = true;
            _connection = connection;
        }

        /// <summary>
        /// Requests admin information for the server.
        /// </summary>
        /// <param name="server">Optional. The server to request information for. Requests information for the current server if this parameter is not defined.</param>
        public void SendADMIN(string server = null)
        {
            if (string.IsNullOrEmpty(server))
                SendRaw("ADMIN");
            else
                SendRaw("ADMIN", server);
        }

        /// <summary>
        /// Sets or clears the client's AWAY status.
        /// </summary>
        /// <param name="reason">Optional. Define to set the AWAY status and leave blank to clear it.</param>
        public void SendAWAY(string reason = null)
        {
            if (string.IsNullOrEmpty(reason))
                SendRaw("AWAY");
            else
                SendRaw("AWAY", GetLimitAppliedParameter(reason, _connection.Support.MaxAwayLength));
        }

        /// <summary>
        /// Sends oper command which connects the current server to another server on the network.
        /// </summary>
        /// <param name="server">The server to connect the current server to.</param>
        public void SendCONNECT(string server)
        {
            SendRaw("CONNECT", server);
        }

        /// <summary>
        /// Requests information about the IRC daemon that the current server is running on.
        /// </summary>
        public void SendINFO()
        {
            SendRaw("INFO");
        }

        /// <summary>
        /// Invites a user into a given channel.
        /// </summary>
        /// <param name="channel">The channel to invite the user into.</param>
        /// <param name="user">The user to invite.</param>
        public void SendINVITE(IRCChannel channel, IRCUser user)
        {
            SendINVITE(channel.Name, user.Nickname);
        }

        /// <summary>
        /// Invites a user into a given channel.
        /// </summary>
        /// <param name="channel">The channel to invite the user into.</param>
        /// <param name="nickname">The nickname of the user to invite.</param>
        public void SendINVITE(IRCChannel channel, string nickname)
        {
            SendINVITE(channel.Name, nickname);
        }

        /// <summary>
        /// Invites a user into a given channel.
        /// </summary>
        /// <param name="channel">The channel to invite the user into.</param>
        /// <param name="nickname">The nickname of the user to invite.</param>
        public void SendINVITE(string channel, string nickname)
        {
            SendRaw("INVITE", nickname, channel);
        }

        /// <summary>
        /// Checks whether a list of users is online.
        /// </summary>
        /// <param name="nicknames">The users to check.</param>
        public void SendISON(IEnumerable<string> nicknames)
        {
            SendRaw("ISON", string.Join(" ", nicknames));
        }

        /// <summary>
        /// Checks whether a nuser is online.
        /// </summary>
        /// <param name="nickname">The user to check.</param>
        public void SendISON(string nickname)
        {
            SendISON(new List<string>() { nickname });
        }

        /// <summary>
        /// Joins a given channel.
        /// </summary>
        /// <param name="channel">The channel that will be joined.</param>
        /// <param name="key">Optional. A password if the channel requires one.</param>
        public void SendJOIN(string channel, string key = null)
        {
            int maxChannels = _connection.Support.MaxNumberOfChannels;
            if (maxChannels > 0 && _connection.Channels.Count >= maxChannels)
                return;

            channel = GetLimitAppliedParameter(channel, _connection.Support.MaxChannelNameLength);

            if (!_connection.Support.ChannelTypes.Contains(channel[0]))
                channel = $"{_connection.Support.ChannelTypes.First()}{channel}";

            if (string.IsNullOrEmpty(key))
                SendRaw("JOIN", channel, key);
            else
                SendRaw("JOIN", channel);
        }

        /// <summary>
        /// Kicks a user from a given channel.
        /// </summary>
        /// <param name="channel">The channel to kick the user from.</param>
        /// <param name="user">The user that should be kicked.</param>
        /// <param name="reason">Optional. The reason for kicking the user.</param>
        public void SendKICK(IRCChannel channel, IRCUser user, string reason = null)
        {
            SendKICK(channel.Name, user.Nickname);
        }

        /// <summary>
        /// Kicks a user from a given channel.
        /// </summary>
        /// <param name="channel">The channel to kick the user from.</param>
        /// <param name="nickname">The nickname of the user that should be kicked.</param>
        /// <param name="reason">Optional. The reason for kicking the user.</param>
        public void SendKICK(IRCChannel channel, string nickname, string reason = null)
        {
            SendKICK(channel.Name, nickname);
        }

        /// <summary>
        /// Kicks a user from a given channel.
        /// </summary>
        /// <param name="channel">The channel to kick the user from.</param>
        /// <param name="nickname">The nickname of the user that should be kicked.</param>
        /// <param name="reason">Optional. The reason for kicking the user.</param>
        public void SendKICK(string channel, string nickname, string reason = null)
        {
            if (string.IsNullOrEmpty(reason))
                SendRaw("KICK", channel, nickname);
            else
                SendRaw("KICK", channel, GetLimitAppliedParameter(reason, _connection.Support.MaxKickReasonLength));
        }

        /// <summary>
        /// Oper command to remove a given user from the server.
        /// </summary>
        /// <param name="user">The user that should be disconnected.</param>
        /// <param name="reason">The reason for disconnecting the user.</param>
        public void SendKILL(IRCUser user, string reason)
        {
            SendKILL(user.Nickname, reason);
        }

        /// <summary>
        /// Oper command to remove a given user from the server.
        /// </summary>
        /// <param name="nickname">The nickname of the user that should be disconnected.</param>
        /// <param name="reason">The reason for disconnecting the user.</param>
        public void SendKILL(string nickname, string reason)
        {
            SendRaw("KILL", nickname, reason);
        }

        /// <summary>
        /// Requests the network's server links.
        /// </summary>
        public void SendLINKS()
        {
            SendRaw("LINKS");
        }

        /// <summary>
        /// Requests a list of channels on the server.
        /// </summary>
        /// <param name="searchString">Optional. A wildcard supported search string to find specific channels.</param>
        public void SendLIST(string searchString = null)
        {
            SendLIST(new List<string> { searchString });
        }

        /// <summary>
        /// Requests a list of channels on the server.
        /// </summary>
        /// <param name="searchStrings">Optional. A wildcard supported list of search strings to find specific channels.</param>
        public void SendLIST(IEnumerable<string> searchStrings = null)
        {
            if (searchStrings?.Any() == true)
                SendRaw("LIST", string.Join(",", searchStrings));
            else
                SendRaw("LIST");
        }

        /// <summary>
        /// Requests server and network wide user and channel numbers.
        /// </summary>
        public void SendLUSERS()
        {
            SendRaw("LUSERS");
        }

        /// <summary>
        /// Requests the modes that are currenetly set on a given channel.
        /// </summary>
        /// <param name="channel">The channel to checked.</param>
        public void SendMODE(string channel)
        {
            SendRaw("MODE", channel);
        }

        /// <summary>
        /// Sets modes on a giver user or channel.
        /// </summary>
        /// <param name="target">The user or channel that will have its modes changed.</param>
        /// <param name="modeString">The mode changes that are made.</param>
        public void SendMODE(string target, ModeString modeString)
        {
            foreach (ModeString modeStr in modeString.Split(_connection.Support.MaxModeChanges))
                SendRaw("MODE", target, modeStr.ToString());
        }

        /// <summary>
        /// Sets modes on a given channel.
        /// </summary>
        /// <param name="channel">The channel that will have its modes changed.</param>
        /// <param name="modes">The modes that will be changed.</param>
        /// <param name="parameters">The parameters that are needed for the changed modes.</param>
        public void SendMODE(IRCChannel channel, string modes, IEnumerable<string> parameters)
        {
            SendMODE(channel.Name, modes, parameters, MessageTarget.Channel);
        }

        /// <summary>
        /// Sets modes on a giver user.
        /// </summary>
        /// <param name="user">The user that will have their modes changed.</param>
        /// <param name="modes">The modes that will be changed.</param>
        /// <param name="parameters">The parameters that are needed for the changed modes.</param>
        public void SendMODE(IRCUser user, string modes, IEnumerable<string> parameters)
        {
            SendMODE(user.Nickname, modes, parameters, MessageTarget.User);
        }

        /// <summary>
        /// Sets modes on a giver user or channel.
        /// </summary>
        /// <param name="target">The user or channel that will have its modes changed</param>
        /// <param name="modes">The modes that will be changed.</param>
        /// <param name="parameters">The parameters that are needed for the changed modes.</param>
        /// <param name="targetType">Whether the change is targetting a user or a channel.</param>
        public void SendMODE(string target, string modes, IEnumerable<string> parameters, MessageTarget targetType)
        {
            ModeString modeString;
            if (targetType == MessageTarget.Channel)
                modeString = new ModeString(modes, parameters.ToList(), _connection.Support.ChannelModes, _connection.Support.StatusModes.Keys.ToList());
            else
                modeString = new ModeString(modes, parameters.ToList(), _connection.Support.UserModes, null);
            SendMODE(target, modeString);
        }

        /// <summary>
        /// Requests the message of the day for the current server or a given server on the network.
        /// </summary>
        /// <param name="server">Optional. The server to request the message of the day of. Requests the message of the day for the current server if undefined.</param>
        public void SendMOTD(string server = null)
        {
            if (string.IsNullOrEmpty(server))
                SendRaw("MOTD");
            else
                SendRaw("MOTD", server);
        }

        /// <summary>
        /// Requests the names of all users in a given channel.
        /// </summary>
        /// <param name="channel">The channel to request the users of.</param>
        public void SendNAMES(string channel)
        {
            SendNAMES(new List<string> { channel });
        }

        /// <summary>
        /// Requests the names of all users in given channels.
        /// </summary>
        /// <param name="channels">The channels to request the users of.</param>
        public void SendNAMES(IEnumerable<string> channels)
        {
            SendRaw("NAMES", string.Join(",", channels));
        }

        /// <summary>
        /// Requests the names of all users in a given channel.
        /// </summary>
        /// <param name="channel">The channel to request the users of.</param>
        public void SendNAMES(IRCChannel channel)
        {
            SendNAMES(channel.Name);
        }

        /// <summary>
        /// Requests the names of all users in given channels.
        /// </summary>
        /// <param name="channels">The channels to request the users of.</param>
        public void SendNAMES(IEnumerable<IRCChannel> channels)
        {
            SendNAMES(channels.Select(x => x.Name));
        }

        /// <summary>
        /// Changes the client's current nickname.
        /// </summary>
        /// <param name="nickname">The new nickname.</param>
        public void SendNICK(string nickname)
        {
            SendRaw("NICK", GetLimitAppliedParameter(nickname, _connection.Support.MaxNickLength));
        }

        /// <summary>
        /// Sends a notice message to a given user or channel.
        /// </summary>
        /// <param name="target">The given user or channel.</param>
        /// <param name="message">The message to be sent.</param>
        public void SendNOTICE(string target, string message)
        {
            SendRaw("NOTICE", target, message);
        }

        /// <summary>
        /// Sends a notice message to a given channel.
        /// </summary>
        /// <param name="channel">The given channel.</param>
        /// <param name="message">The message to be sent.</param>
        public void SendNOTICE(IRCChannel channel, string message)
        {
            SendPRIVMSG(channel.Name, message);
        }

        /// <summary>
        /// Sends a notice message to a given user.
        /// </summary>
        /// <param name="user">The given user.</param>
        /// <param name="message">The message to be sent.</param>
        public void SendNOTICE(IRCUser user, string message)
        {
            SendPRIVMSG(user.Nickname, message);
        }

        /// <summary>
        /// Leaves a given channel.
        /// </summary>
        /// <param name="channel">The channel that should be parted.</param>
        /// <param name="reason">Optional. The reason for leaving the channel.</param>
        public void SendPART(IRCChannel channel, string reason = null)
        {
            SendPART(channel.Name, reason);
        }

        /// <summary>
        /// Leaves a given channel.
        /// </summary>
        /// <param name="channel">The channel that should be parted.</param>
        /// <param name="reason">Optional. The reason for leaving the channel.</param>
        public void SendPART(string channel, string reason = null)
        {
            if (string.IsNullOrEmpty(reason))
                SendRaw("PART", channel);
            else
                SendRaw("PART", channel, reason);
        }

        /// <summary>
        /// Sends a password required for connecting to the server.
        /// </summary>
        /// <param name="password">The password that is required for the server connection.</param>
        public void SendPASS(string password)
        {
            SendRaw("PASS", password);
        }

        /// <summary>
        /// Sends a PING message to the server.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public void SendPING(string message)
        {
            SendRaw("PONG", message);
        }

        /// <summary>
        /// Replies to a PING message from the server.
        /// </summary>
        /// <param name="reply">The reply that was included in the PING message.</param>
        public void SendPONG(string reply)
        {
            SendRaw("PONG", reply);
        }

        /// <summary>
        /// Sends a standard message to a given user or channel.
        /// </summary>
        /// <param name="target">The given user or channel.</param>
        /// <param name="message">The message to be sent.</param>
        public void SendPRIVMSG(string target, string message)
        {
            SendRaw("PRIVMSG", target, message);
        }

        /// <summary>
        /// Sends a standard message to a given channel.
        /// </summary>
        /// <param name="channel">The given channel.</param>
        /// <param name="message">The message to be sent.</param>
        public void SendPRIVMSG(IRCChannel channel, string message)
        {
            SendPRIVMSG(channel.Name, message);
        }

        /// <summary>
        /// Sends a standard message to a given user.
        /// </summary>
        /// <param name="user">The given user.</param>
        /// <param name="message">The message to be sent.</param>
        public void SendPRIVMSG(IRCUser user, string message)
        {
            SendPRIVMSG(user.Nickname, message);
        }

        /// <summary>
        /// Quits the connection to the server.
        /// </summary>
        /// <param name="message">Optional. A message to be displayed on disconnect.</param>
        public void SendQUIT(string message = null)
        {
            if (string.IsNullOrEmpty(message))
                SendRaw("QUIT");
            else
                SendRaw("QUIT", message);
        }

        /// <summary>
        /// Sends a raw IRC line to the server.
        /// </summary>
        /// <param name="parameters">The parts of the message that will be sent. A space will automatically be inserted between these parameters.</param>
        public void SendRaw(params string[] parameters)
        {
            if (!parameters.Any())
                return;

            string lastParam = parameters.Last();
            if (lastParam.Contains(' '))
                parameters[parameters.Length - 1] = $":{lastParam}";

            string lineToSend = string.Join(" ", parameters);
            IRCMessage message = new IRCMessage(lineToSend);
            IRCMessageCancelEventArgs args = new IRCMessageCancelEventArgs(message);
            IRCEvents.OnMessageSending(this, args);
            if (args.Cancel)
                return;

            _outputStream.Write($"{lineToSend}\r\n");
            IRCEvents.OnMessageSent(this, new IRCMessageEventArgs(message));
        }

        /// <summary>
        /// Sets a channel's topic.
        /// </summary>
        /// <param name="channel">The channel in which the topic has to be changed.</param>
        /// <param name="topic">The new topic.</param>
        public void SendTOPIC(string channel, string topic)
        {
            SendRaw("TOPIC", channel, GetLimitAppliedParameter(topic, _connection.Support.MaxTopicLength));
        }

        /// <summary>
        /// Logs in on the server.
        /// </summary>
        /// <param name="ident">The ident or username that will be used on the server.</param>
        /// <param name="gecos">The gecos or realname that will be used on the server.</param>
        public void SendUSER(string ident, string gecos)
        {
            SendRaw("USER", ident, "*", "*", gecos);
        }

        /// <summary>
        /// Requests WHO information for a given user, channel or search string.
        /// </summary>
        /// <param name="target">The target to request WHO information for.</param>
        public void SendWHO(string target)
        {
            SendRaw("WHO", target);
        }

        private string GetLimitAppliedParameter(string param, int maxLength)
        {
            if (maxLength > 0 && param.Length > maxLength)
                param = param.Substring(0, maxLength);

            return param;
        }

        #region IDisposable Support

        /// <summary>
        /// Disposable pattern implementation.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposable pattern implementation.
        /// </summary>
        /// <param name="disposing">Should dispose Y/N?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    _outputStream.Dispose();

                _disposedValue = true;
            }
        }

        #endregion IDisposable Support
    }
}
