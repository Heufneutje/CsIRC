using System;
using System.Collections.Generic;

namespace CsIRC.Core
{
    /// <summary>
    /// An IRC channel the client has joined.
    /// </summary>
    public class IRCChannel
    {
        /// <summary>
        /// The IRC channel's name including the channel prefix character.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A dictionary that maps IRC users to their nickname.
        /// </summary>
        public Dictionary<IRCUser, string> Users { get; private set; }

        /// <summary>
        /// A dictionary containing all modes currently set in the channel and their parameters.
        /// </summary>
        public Dictionary<string, string> Modes { get; private set; }

        /// <summary>
        /// The channel's current topic.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// The hostmask of the user that set the channel topic.
        /// </summary>
        public IRCHostmask TopicSetter { get; set; }

        /// <summary>
        /// The date and time on which the current topic was set.
        /// </summary>
        public DateTime TopicSetDate { get; set; }

        /// <summary>
        /// The date and time on which the channel was created.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Whether or not the channel's user list has been fully retrieved.
        /// </summary>
        public bool UserlistComplete { get; set; }

        /// <summary>
        /// Creates a new IRC channel instance with a given name and initializes the user and modes dictionaries.
        /// </summary>
        /// <param name="name">The IRC channel's name including the channel prefix character.</param>
        public IRCChannel(string name)
        {
            Name = name;
            Users = new Dictionary<IRCUser, string>();
            Modes = new Dictionary<string, string>();
            UserlistComplete = true;
        }

        /// <summary>
        /// Override for string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
