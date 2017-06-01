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
        public string Username { get; set; }

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
        /// The user's current away message.
        /// </summary>
        public string AwayMessage { get; set; }

        /// <summary>
        /// Creates a new IRC user instance from a nickname.
        /// </summary>
        /// <param name="nickname">The user's nickname.</param>
        public IRCUser(string nickname)
        {
            Nickname = nickname;
        }

        /// <summary>
        /// Creates a new IRC user instance from a hostmask.
        /// </summary>
        /// <param name="hostmask">The user's hostmask.</param>
        public IRCUser(IRCHostmask hostmask)
        {
            Nickname = hostmask.Nickname;
            Username = hostmask.Username;
            Hostname = hostmask.Hostname;
        }

        /// <summary>
        /// Get the user's hostmask.
        /// </summary>
        /// <returns>The user's hostmask.</returns>
        public IRCHostmask GetHostmask()
        {
            return new IRCHostmask(ToString());
        }

        /// <summary>
        /// Override for string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Username))
                return Nickname;
            return $"{Nickname}!{Username}@{Hostname}";
        }
    }
}
