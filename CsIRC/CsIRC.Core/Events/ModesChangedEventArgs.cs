namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for when the modes on a user or channel are changed.
    /// </summary>
    public class ModesChangedEventArgs : ChannelUserCommandEventArgs
    {
        /// <summary>
        /// Whether the message targets a user or a channel.
        /// </summary>
        public MessageTarget Target { get; private set; }

        /// <summary>
        /// The modes that have been changed.
        /// </summary>
        public ModeString ModeString { get; private set; }

        /// <summary>
        /// Creates new event arguments for when the modes on a channel are changed.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="channel">The channel the event applies to.</param>
        /// <param name="modeString">Whether the message targets a user or a channel.</param>
        public ModesChangedEventArgs(IRCMessage message, IRCChannel channel, ModeString modeString) : base(message, channel, null)
        {
            ModeString = modeString;
            Target = MessageTarget.Channel;
        }

        /// <summary>
        /// Creates new event arguments for when the modes on a user are changed.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="user">The user the event applies to.</param>
        /// <param name="modeString">Whether the message targets a user or a channel.</param>
        public ModesChangedEventArgs(IRCMessage message, IRCUser user, ModeString modeString) : base(message, null, user)
        {
            ModeString = modeString;
            Target = MessageTarget.User;
        }
    }
}
