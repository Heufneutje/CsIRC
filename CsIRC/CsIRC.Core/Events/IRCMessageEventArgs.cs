using System;

namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for an IRC message.
    /// </summary>
    public class IRCMessageEventArgs : EventArgs
    {
        /// <summary>
        /// The message that was received from the server.
        /// </summary>
        public IRCMessage Message { get; private set; }

        /// <summary>
        /// Creates new event arguments for an IRC message.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        public IRCMessageEventArgs(IRCMessage message)
        {
            Message = message;
        }
    }
}
