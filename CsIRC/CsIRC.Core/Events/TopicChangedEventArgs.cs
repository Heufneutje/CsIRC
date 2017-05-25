namespace CsIRC.Core.Events
{
    /// <summary>
    /// Event arguments for when a channel's topic is changed.
    /// </summary>
    public class TopicChangedEventArgs : ChannelUserCommandEventArgs
    {
        /// <summary>
        /// The channel's old topic.
        /// </summary>
        public string OldTopic { get; private set; }

        /// <summary>
        /// The channel's new topic.
        /// </summary>
        public string NewTopic { get; private set; }

        /// <summary>
        /// Creates new event arguments for when a channel's topic is changed.
        /// </summary>
        /// <param name="message">The message that was received from the server.</param>
        /// <param name="channel">The IRC channel the event applies to.</param>
        /// <param name="user">The IRC user the event applies to.</param>
        /// <param name="oldTopic">The channel's old topic.</param>
        /// <param name="newTopic">The channel's new topic.</param>
        public TopicChangedEventArgs(IRCMessage message, IRCChannel channel, IRCUser user, string oldTopic, string newTopic) : base(message, channel, user)
        {
            OldTopic = oldTopic;
            NewTopic = newTopic;
        }
    }
}
