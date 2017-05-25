using System.ComponentModel;

namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for an IRC message that can be canceled.
    /// </summary>
    public class IRCMessageCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        /// The message that was received from the server.
        /// </summary>
        public IRCMessage Message { get; private set; }

        /// <summary>
        /// Creates new event arguments for an IRC message that can be canceled.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        public IRCMessageCancelEventArgs(IRCMessage message)
        {
            Message = message;
        }
    }
}
