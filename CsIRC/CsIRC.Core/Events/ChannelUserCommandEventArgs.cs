namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for an event that applies to a user in a channel.
    /// </summary>
    public class ChannelUserCommandEventArgs : IRCMessageEventArgs, IChannelEvent, IUserEvent
    {
        /// <summary>
        /// The IRC channel the event applies to.
        /// </summary>
        public IRCChannel Channel { get; private set; }

        /// <summary>
        /// The IRC user the event applies to.
        /// </summary>
        public IRCUser User { get; private set; }

        /// <summary>
        /// Creates new event arguments for an event that applies to a user in a channel.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="channel">The channel the event applies to.</param>
        /// <param name="user">The user the event applies to.</param>
        public ChannelUserCommandEventArgs(IRCMessage message, IRCChannel channel, IRCUser user) : base(message)
        {
            Channel = channel;
            User = user;
        }
    }
}
