using System;
using System.Collections.Generic;
using System.Linq;

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
        /// The date and time the message is received. Will default to current time. IRCv3 server-time will be used if available.
        /// </summary>
        public DateTime ReceivedAt { get; private set; }

        /// <summary>
        /// Creates a new instance of a received IRC message from a raw IRC line.
        /// </summary>
        /// <param name="rawLine">The raw IRC line that was received from the server.</param>
        public IRCMessage(string rawLine)
        {
            // Based on https://github.com/ElementalAlchemist/txircd/blob/93f949e297e78a802932c239a6c942cb1c3b8b50/txircd/ircbase.py#L12-L50
            RawLine = rawLine;
            ReceivedAt = DateTime.Now;

            string line = rawLine.Replace("\0", "");
            if (line.Length == 0)
                return;

            Dictionary<string, string> tags = new Dictionary<string, string>();
            if (line[0] == '@')
            {
                if (!line.Contains(' '))
                    return;

                string[] tagSplit = line.Split(new char[] { ' ' }, 2);
                line = tagSplit[1];
                tags = ParseTags(tagSplit[0].Substring(1));
            }

            string prefix = null;
            if (line[0] == ':')
            {
                if (!line.Contains(' '))
                    return;

                string[] lineSplit = line.Split(new char[] { ' ' }, 2);
                prefix = lineSplit[0].Substring(1);
                line = lineSplit[1];
            }

            string linePart;
            string lastParam;
            if (line.Contains(" :"))
            {
                string[] lineSplit = line.Split(new string[] { " :" }, 2, StringSplitOptions.None);
                linePart = lineSplit[0];
                lastParam = lineSplit[1];
            }
            else
            {
                linePart = line;
                lastParam = null;
            }

            if (string.IsNullOrEmpty(linePart))
                return;

            string command;
            List<string> parameters;
            if (linePart.Contains(' '))
            {
                string[] lineSplit = linePart.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                command = lineSplit[0];
                parameters = lineSplit.Skip(1).ToList();
            }
            else
            {
                command = linePart;
                parameters = new List<string>();
            }

            if (lastParam != null)
                parameters.Add(lastParam);

            Prefix = prefix;
            Command = command;
            Parameters = parameters;
            Tags = tags;
        }

        private static Dictionary<string, string> ParseTags(string tagLine)
        {
            // Based on https://github.com/ElementalAlchemist/txircd/blob/93f949e297e78a802932c239a6c942cb1c3b8b50/txircd/ircbase.py#L52-L86
            Dictionary<string, string> tags = new Dictionary<string, string>();
            foreach (string tagValue in tagLine.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string tag;
                string value;
                if (tagValue.Contains('='))
                {
                    string[] tagSplit = tagValue.Split(new char[] { ';' }, 2);
                    tag = tagSplit[0];
                    bool isEscaped = false;
                    List<char> valueChars = new List<char>();
                    foreach (char character in tagSplit[1])
                    {
                        if (isEscaped)
                        {
                            switch (character)
                            {
                                case '\\':
                                    valueChars.Add('\\');
                                    break;
                                case ':':
                                    valueChars.Add(';');
                                    break;
                                case 'r':
                                    valueChars.Add('\r');
                                    break;
                                case 'n':
                                    valueChars.Add('\n');
                                    break;
                                case 's':
                                    valueChars.Add(' ');
                                    break;
                                default:
                                    valueChars.Add(character);
                                    break;
                            }
                            isEscaped = false;
                            continue;
                        }
                        if (character == '\\')
                        {
                            isEscaped = true;
                            continue;
                        }
                        valueChars.Add(character);
                    }
                    value = string.Join("", valueChars);
                }
                else
                {
                    tag = tagValue;
                    value = null;
                }
                tags.Add(tag, value);
            }
            return tags;
        }
    }
}
