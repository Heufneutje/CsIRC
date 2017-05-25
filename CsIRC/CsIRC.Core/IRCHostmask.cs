using System.Linq;

namespace CsIRC.Core
{
    /// <summary>
    /// A hostmask created from a message prefix.
    /// </summary>
    public class IRCHostmask
    {
        /// <summary>
        /// The nickname part of the hostmask.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The ident or username part of the hostmask.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The hostname part of the hostmask.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Creates a hostmask from a user's prefix.
        /// </summary>
        /// <param name="userPrefix">The user's prefix.</param>
        public IRCHostmask(string userPrefix)
        {
            if (string.IsNullOrEmpty(userPrefix))
                Nickname = string.Empty;
            else if (userPrefix.Contains('!'))
            {
                int excIndex = userPrefix.IndexOf('!');
                int atIndex = userPrefix.IndexOf('@');

                Nickname = userPrefix.Substring(0, excIndex);
                Username = userPrefix.Substring(excIndex + 1, atIndex - excIndex - 1);
                Hostname = userPrefix.Substring(atIndex + 1);
            }
            else
                Nickname = userPrefix;
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
