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
    /// Handler for events that contain a PRIVMSG or NOTICE command.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void MessageCommandHandler(object sender, MessageCommandEventArgs args);

    /// <summary>
    /// Handler for events that apply to a user in a channel.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void ChannelUserCommandHandler(object sender, ChannelUserCommandEventArgs args);

    /// <summary>
    /// Handler for events that apply to a user in a channel with a reason.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void ChannelUserReasonCommandHandler(object sender, ChannelUserReasonCommandEventArgs args);

    /// <summary>
    /// Handler for events where a user changes nicknames.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void NickChangedHandler(object sender, NickChangedEventArgs args);

    /// <summary>
    /// Handler for events where a channel's topic is changed.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void TopicChangedHandler(object sender, TopicChangedEventArgs args);

    /// <summary>
    /// Handler for events where a channel's userlist is updated.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void UserlistUpdatedHandler(object sender, UserlistUpdatedEventArgs args);

    /// <summary>
    /// Handler for events where the modes on a user or channel are changed.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void ModesChangedHandler(object sender, ModesChangedEventArgs args);

    /// <summary>
    /// Handler for events that apply to a user with a reason.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void UserReasonCommandHandler(object sender, UserCommandReasonEventArgs args);

    /// <summary>
    /// Handler for events where a user is kicked from a channel.
    /// </summary>
    /// <param name="sender">The core part or plugin that sent the event.</param>
    /// <param name="args">The arguments of the event.</param>
    public delegate void UserKickedHandler(object sender, UserKickedEventArgs args);
}
