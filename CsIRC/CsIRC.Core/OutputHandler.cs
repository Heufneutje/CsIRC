using System;
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
        private StreamWriter _outputStream;
        private IRCConnection _connection;
        private bool _disposedValue; // IDisposable support. To detect redundant calls.

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
        /// Joins a given channel.
        /// </summary>
        /// <param name="channel">The channel that will be joined.</param>
        /// <param name="key">Optional. A password if the channel requires one.</param>
        public void SendJOIN(string channel, string key = null)
        {
            if (!_connection.Support.ChannelTypes.Contains(channel[0]))
                channel = $"{_connection.Support.ChannelTypes.First()}{channel}";

            if (string.IsNullOrEmpty(key))
                SendRaw("JOIN", channel, key);
            else
                SendRaw("JOIN", channel);
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
        /// Changes the client's current nickname.
        /// </summary>
        /// <param name="nickname">The new nickname.</param>
        public void SendNICK(string nickname)
        {
            SendRaw("NICK", nickname);
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
                    _outputStream.Dispose();

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
        #endregion

    }
}
