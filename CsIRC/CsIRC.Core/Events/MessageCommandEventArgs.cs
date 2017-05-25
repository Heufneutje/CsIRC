using System.Linq;

namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for a message containing a PRIVMSG or NOTICE command.
    /// </summary>
    public class MessageCommandEventArgs : ChannelUserCommandEventArgs
    {
        /// <summary>
        /// Whether the message targets a user or a channel.
        /// </summary>
        public MessageTarget Target { get; private set; }

        /// <summary>
        /// The type of message command (PRIVMSG or NOTICE).
        /// </summary>
        public MessageCommandType CommandType { get; private set; }

        private string _messageBody;

        /// <summary>
        /// The text body of the message.
        /// </summary>
        public string MessageBody
        {
            get
            {
                if (_messageBody == null)
                    _messageBody = string.Join(" ", Message.Parameters.Skip(1));

                return _messageBody;
            }
        }

        /// <summary>
        /// Creates new event arguments for a message containing a PRIVMSG or NOTICE command sent to a channel.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="channel">The IRC channel the event applies to.</param>
        /// <param name="user">The IRC user the event applies to.</param>
        public MessageCommandEventArgs(IRCMessage message, IRCChannel channel, IRCUser user) : base(message, channel, user)
        {
            Target = MessageTarget.Channel;
            SetCommandType();
        }

        /// <summary>
        /// Creates new event arguments for a message containing a PRIVMSG or NOTICE command sent to a user.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="user">The IRC user the event applies to.</param>
        public MessageCommandEventArgs(IRCMessage message, IRCUser user) : base(message, null, user)
        {
            Target = MessageTarget.User;
            SetCommandType();
        }

        private void SetCommandType()
        {
            switch (Message.Command)
            {
                case "NOTICE":
                    CommandType = MessageCommandType.Notice;
                    break;
                case "PRIVMSG":
                    CommandType = MessageCommandType.Privmsg;
                    break;
            }
        }
    }
}
