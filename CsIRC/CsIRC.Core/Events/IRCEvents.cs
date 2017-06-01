namespace CsIRC.Core.Events
{
    /// <summary>
    /// Static class witha all possible events that can be fired from the client.
    /// </summary>
    public static class IRCEvents
    {
        #region Events

        /// <summary>
        /// Event which is fired after a message is received from the server and before the message has been parsed by the client.
        /// Cancelling this event will prevent the client from parsing it, allowing for custom handling logic.
        /// </summary>
        public static event CancelableMessageHandler MessageReceiving;

        /// <summary>
        /// Event which is fired after a message is received from the server and after the message has been parsed by the client.
        /// </summary>
        public static event MessageHandler MessageReceived;

        /// <summary>
        /// Event which is fired before a message is sent to the server.
        /// Cancelling this event will prevent the client from sending it, allowing for custom handling logic.
        /// </summary>
        public static event CancelableMessageHandler MessageSending;

        /// <summary>
        /// Event which is fired after a message is sent to the server.
        /// </summary>
        public static event MessageHandler MessageSent;

        /// <summary>
        /// Event which is fired when a PRIVMSG or NOTICE command is received.
        /// </summary>
        public static event MessageCommandHandler MessageCommandReceived;

        /// <summary>
        /// Event which is fired when a channel is joined.
        /// </summary>
        public static event ChannelUserCommandHandler ChannelJoined;

        /// <summary>
        /// Event which is fired when a channel is parted.
        /// </summary>
        public static event ChannelUserReasonCommandHandler ChannelParted;

        /// <summary>
        /// Event which is fired when a user changes nicknames.
        /// </summary>
        public static event NickChangedHandler NicknameChanged;

        /// <summary>
        /// Event which is fired when a channel's topic is changed.
        /// </summary>
        public static event TopicChangedHandler TopicChanged;

        /// <summary>
        /// Event which is fired when a channel's userlist is updated.
        /// </summary>
        public static event UserlistUpdatedHandler UserlistUpdated;

        /// <summary>
        /// Event which is fired when the client is invited into a channel.
        /// </summary>
        public static event ChannelUserCommandHandler InvitedIntoChannel;

        /// <summary>
        /// Event which is fired when a channel's or user's modes are changed.
        /// </summary>
        public static event ModesChangedHandler ModesChanged;

        /// <summary>
        /// Event which is fired when a user leaves the server.
        /// </summary>
        public static event UserReasonCommandHandler UserQuit;

        /// <summary>
        /// Event which is fired when a user is kicked from a channel.
        /// </summary>
        public static event UserKickedHandler UserKicked;

        /// <summary>
        /// Event which is fired when a user's away status is set or removed. Requires IRCv3 away-notify to be enabled.
        /// </summary>
        public static event UserReasonCommandHandler UserAwayStatusChanged;

        #endregion Events

        #region Event Invokers

        /// <summary>
        /// Fires event after a message is received from the server and before the message has been parsed by the client.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnMessageReceiving(object sender, IRCMessageCancelEventArgs args)
        {
            MessageReceiving?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event after a message is received from the server and after the message has been parsed by the client.
        /// </summary>
        //// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnMessageReceived(object sender, IRCMessageEventArgs args)
        {
            MessageReceived?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event before a message is sent to the server.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnMessageSending(object sender, IRCMessageCancelEventArgs args)
        {
            MessageSending?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event after a message is sent to the server.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnMessageSent(object sender, IRCMessageEventArgs args)
        {
            MessageSent?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a PRIVMSG or NOTICE command is received.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnMessageCommandReceived(object sender, MessageCommandEventArgs args)
        {
            MessageCommandReceived?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a channel is joined.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnChannelJoined(object sender, ChannelUserCommandEventArgs args)
        {
            ChannelJoined?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a channel is parted.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnChannelParted(object sender, ChannelUserReasonCommandEventArgs args)
        {
            ChannelParted?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a user changes nicknames.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnNicknameChanged(object sender, NickChangedEventArgs args)
        {
            NicknameChanged?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a channel's topic is changed.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnTopicChanged(object sender, TopicChangedEventArgs args)
        {
            TopicChanged?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a channel's userlist is updated.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnUserlistUpdated(object sender, UserlistUpdatedEventArgs args)
        {
            UserlistUpdated?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when the client is invited into a channel.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnInvitedIntoChannel(object sender, ChannelUserCommandEventArgs args)
        {
            InvitedIntoChannel?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a channel's or user's modes are changed.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnModesChanged(object sender, ModesChangedEventArgs args)
        {
            ModesChanged?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a user leaves the server.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnUserQuit(object sender, UserReasonCommandEventArgs args)
        {
            UserQuit.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a user is kicked from a channel.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnUserKicked(object sender, UserKickedEventArgs args)
        {
            UserKicked?.Invoke(sender, args);
        }

        /// <summary>
        /// Fires event when a user's away status is set or removed.
        /// </summary>
        /// <param name="sender">The core part or plugin that sent the event.</param>
        /// <param name="args">The arguments of the event.</param>
        public static void OnUserAwayStatusChanged(object sender, UserReasonCommandEventArgs args)
        {
            UserAwayStatusChanged?.Invoke(sender, args);
        }

        #endregion Event Invokers
    }
}
