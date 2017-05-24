using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CsIRC.Core.Events;

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
            IRCMessage message = ParsingUtils.ParseRawIRCLine((string)e.UserState);
            IRCMessageCancelEventArgs args = new IRCMessageCancelEventArgs(message);
            IRCEvents.OnMessageReceiving(this, args);
            if (args.Cancel)
                return;

            if (byte.TryParse(message.Command, out byte numeric))
                HandleNumeric(message);
            else
                HandleCommand(message);

            IRCEvents.OnMessageReceived(this, new IRCMessageEventArgs(message));
        }

        private void HandleCommand(IRCMessage message)
        {
            switch (message.Command)
            {
                case "PING":
                    _connection.Output.SendPONG(message.Parameters.First());
                    break;
            }
        }

        private void HandleNumeric(IRCMessage message)
        {
            switch (message.Command)
            {
                case "001":
                    _connection.IsConnected = true;
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
        #endregion
    }
}
