using System.Collections.Generic;

namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for when a channel's userlist is updated.
    /// </summary>
    public class UserlistUpdatedEventArgs : IRCMessageEventArgs, IChannelEvent
    {
        /// <summary>
        /// The IRC channel the event applies to.
        /// </summary>
        public IRCChannel Channel { get; private set; }

        /// <summary>
        /// The users whose information has been updated.
        /// </summary>
        public List<IRCUser> UpdatedUsers { get; private set; }

        /// <summary>
        /// Creates new event arguments for when a channel's userlist is updated.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="channel">The channel the event applies to.</param>
        /// <param name="updatedUsers">The users whose information has been updated.</param>
        public UserlistUpdatedEventArgs(IRCMessage message, IRCChannel channel, List<IRCUser> updatedUsers) : base(message)
        {
            Channel = channel;
            UpdatedUsers = updatedUsers;
        }
    }
}
