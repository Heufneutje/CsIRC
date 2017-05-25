namespace CsIRC.Core.Events
{
    /// <summary>
    /// An interface implemented by events that apply to an IRC user.
    /// </summary>
    public interface IUserEvent
    {
        /// <summary>
        /// The IRC user the event applies to.
        /// </summary>
        IRCUser User { get; }
    }
}
