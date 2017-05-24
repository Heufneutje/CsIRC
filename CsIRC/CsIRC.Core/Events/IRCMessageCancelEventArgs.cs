using System.ComponentModel;

namespace CsIRC.Core.Events
{
    public class IRCMessageCancelEventArgs : CancelEventArgs
    {
        public IRCMessage Message { get; private set; }

        public IRCMessageCancelEventArgs(IRCMessage message)
        {
            Message = message;
        }
    }
}
