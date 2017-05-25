namespace CsIRC.Core.Events
{
    /// <summary>
    /// An interface implemented by events that apply to an IRC channel.
    /// </summary>
    public interface IChannelEvent
    {
        /// <summary>
        /// The IRC channel the event applies to.
        /// </summary>
        IRCChannel Channel { get; }
    }
}
