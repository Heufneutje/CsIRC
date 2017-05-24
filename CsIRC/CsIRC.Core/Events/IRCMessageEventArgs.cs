using System;

namespace CsIRC.Core.Events
{
    public class IRCMessageEventArgs : EventArgs
    {
        public IRCMessage Message { get; private set; }

        public IRCMessageEventArgs(IRCMessage message)
        {
            Message = message;
        }
    }
}
