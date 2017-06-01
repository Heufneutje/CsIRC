namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for when a user is kicked from a channel.
    /// </summary>
    public class UserKickedEventArgs : ChannelUserReasonCommandEventArgs
    {
        /// <summary>
        /// The user that is kicked from the channel.
        /// </summary>
        public IRCUser KickedUser { get; private set; }

        /// <summary>
        /// Creates new event arguments for when a user is kicked from a channel.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="channel">The channel the event applies to.</param>
        /// <param name="kickingUser">The user that used the KICK command.</param>
        /// <param name="kickedUser">The user that was kicked from the channel.</param>
        /// <param name="reason">The optional reason for the kick.</param>
        public UserKickedEventArgs(IRCMessage message, IRCChannel channel, IRCUser kickingUser, IRCUser kickedUser, string reason) : base(message, channel, kickingUser, reason)
        {
            KickedUser = kickedUser;
        }
    }
}
