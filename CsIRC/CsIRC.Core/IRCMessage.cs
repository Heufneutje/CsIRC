using System.Collections.Generic;

namespace CsIRC.Core
{
    /// <summary>
    /// A received IRC message.
    /// </summary>
    public class IRCMessage
    {
        /// <summary>
        /// The raw IRC line that was received from the server.
        /// </summary>
        public string RawLine { get; private set; }

        /// <summary>
        /// The message's prefix.
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// The message's command.
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// The message's parameters.
        /// </summary>
        public List<string> Parameters { get; private set; }

        /// <summary>
        /// The IRCv3 message tags.
        /// </summary>
        public Dictionary<string, string> Tags { get; private set; }

        /// <summary>
        /// Creates a new instance of a received IRC message.
        /// </summary>
        /// <param name="rawLine">The raw IRC line that was received from the server.</param>
        /// <param name="prefix">The message's prefix.</param>
        /// <param name="command">The message's command.</param>
        /// <param name="parameters">The message's parameters.</param>
        /// <param name="tags">The IRCv3 message tags.</param>
        public IRCMessage(string rawLine, string prefix, string command, List<string> parameters, Dictionary<string, string> tags)
        {
            RawLine = rawLine;
            Prefix = prefix;
            Command = command;
            Parameters = parameters;
            Tags = tags;
        }
    }
}
