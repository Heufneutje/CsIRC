namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for commands that apply to a user and a reason.
    /// </summary>
    public class UserCommandReasonEventArgs : IRCMessageEventArgs, IUserEvent
    {
        /// <summary>
        /// he user the event applies to.
        /// </summary>
        public IRCUser User { get; private set; }

        /// <summary>
        /// The optional reason for the event.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Creates new event arguments for commands that apply to a user and a reason.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user">he user the event applies to.</param>
        /// <param name="reason">The optional reason for the event.</param>
        public UserCommandReasonEventArgs(IRCMessage message, IRCUser user, string reason) : base(message)
        {
            User = user;
            Reason = reason;
        }
    }
}
