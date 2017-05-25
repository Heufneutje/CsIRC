namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for when a user changes nicknames.
    /// </summary>
    public class NickChangedEventArgs : IRCMessageEventArgs, IUserEvent
    {
        /// <summary>
        /// The user the event applies to.
        /// </summary>
        public IRCUser User { get; private set; }

        /// <summary>
        /// The user's old nickname.
        /// </summary>
        public string OldNickname { get; private set; }

        /// <summary>
        /// The user's new nickname.
        /// </summary>
        public string NewNickname { get; private set; }

        /// <summary>
        /// Creates new event arguments for when a user changes nicknames.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="user">The user the event applies to.</param>
        /// <param name="oldNickname">The user's old nickname.</param>
        /// <param name="newNickname">The user's new nickname.</param>
        public NickChangedEventArgs(IRCMessage message, IRCUser user, string oldNickname, string newNickname) : base(message)
        {
            User = user;
            OldNickname = oldNickname;
            NewNickname = newNickname;
        }
    }
}
