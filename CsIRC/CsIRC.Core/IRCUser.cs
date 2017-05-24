namespace CsIRC.Core
{
    /// <summary>
    /// An IRC user instance.
    /// </summary>
    public class IRCUser
    {
        /// <summary>
        /// The user's nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The user's ident or username.
        /// </summary>
        public string Usermame { get; set; }

        /// <summary>
        /// The user's hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The user's gecos or realname.
        /// </summary>
        public string Gecos { get; set; }

        /// <summary>
        /// The server the user is connected to.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// THe number of hops to this user.
        /// </summary>
        public uint Hops { get; set; }

        /// <summary>
        /// Whether or not this user is a server/network operator.
        /// </summary>
        public bool IsOper { get; set; }

        /// <summary>
        /// Whether or not this user is currently away.
        /// </summary>
        public bool IsAway { get; set; }

        /// <summary>
        /// Creates a new IRC user instance from a hostmask.
        /// </summary>
        /// <param name="nickname">The user's nickname.</param>
        /// <param name="username">The user's ident or username.</param>
        /// <param name="hostname">The user's hostname.</param>
        public IRCUser(string nickname, string username, string hostname)
        {
            Nickname = nickname;
            Usermame = username;
            Hostname = hostname;
        }

        /// <summary>
        /// Creates a new IRC user instance from a hostmask.
        /// </summary>
        /// <param name="nickname">The user's nickname.</param>
        public IRCUser(string nickname)
        {
            Nickname = nickname;
        }
    }
}
