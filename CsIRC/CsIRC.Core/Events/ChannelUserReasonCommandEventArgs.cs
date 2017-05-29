namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for commands that apply to a user in a channel and a reason.
    /// </summary>
    public class ChannelUserReasonCommandEventArgs : ChannelUserCommandEventArgs
    {
        /// <summary>
        /// The optional reason for the event.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Creates new event arguments for commands that apply to a user in a channel and a reason.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="channel">The channel the event applies to.</param>
        /// <param name="user">The user the event applies to.</param>
        /// <param name="reason">The reason for parting the channel.</param>
        public ChannelUserReasonCommandEventArgs(IRCMessage message, IRCChannel channel, IRCUser user, string reason) : base(message, channel, user)
        {
            Reason = reason;
        }
    }
}
