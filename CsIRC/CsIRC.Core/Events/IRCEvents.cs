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
        #endregion

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
        #endregion
    }
}
