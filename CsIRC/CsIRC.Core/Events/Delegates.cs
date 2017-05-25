namespace CsIRC.Core.Events
{
    /// <summary>
    /// Handler for events that contain an IRC message.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void MessageHandler(object sender, IRCMessageEventArgs args);

    /// <summary>
    /// Handler for events that contain an IRC message and can be canceled.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void CancelableMessageHandler(object sender, IRCMessageCancelEventArgs args);

    /// <summary>
    /// Handler for events that contain a PRIVMSG command.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void MessageCommandHandler(object sender, MessageCommandEventArgs args);

    /// <summary>
    /// Handler for events that occur when joining a channel.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void ChannelUserCommandHandler(object sender, ChannelUserCommandEventArgs args);
}
