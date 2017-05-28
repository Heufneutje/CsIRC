namespace CsIRC.Core
{
    public enum MessageTarget
    {
        User = 0,
        Channel = 1
    }

    public enum ModeType
    {
        List = 0,
        ParamUnset = 1,
        ParamSet = 2,
        NoParam = 3,
        Status = 4
    }

    public enum MessageCommandType
    {
        Privmsg = 0,
        Notice = 1
    }
}
