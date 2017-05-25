using System;
using System.Collections.Generic;
using System.Linq;

namespace CsIRC.Core
{
    /// <summary>
    /// Helper class for parsing raw IRC messages.
    /// </summary>
    public static class ParsingUtils
    {
        /// <summary>
        /// Parses a raw IRC line into a convenient format.
        /// </summary>
        /// <param name="rawLine">The given raw IRC line.</param>
        /// <returns>The parsed message.</returns>
        public static IRCMessage ParseRawIRCLine(string rawLine)
        {
            // Based on https://github.com/ElementalAlchemist/txircd/blob/93f949e297e78a802932c239a6c942cb1c3b8b50/txircd/ircbase.py#L12-L50
            string line = rawLine.Replace("\0", "");
            if (line.Length == 0)
                return null;

            Dictionary<string, string> tags = new Dictionary<string, string>();
            if (line[0] == '@')
            {
                if (!line.Contains(' '))
                    return null;

                string[] tagSplit = line.Split(new char[] { ' ' }, 2);
                line = tagSplit[1];
                tags = ParseTags(tagSplit[0].Substring(1));
            }

            string prefix = null;
            if (line[0] == ':')
            {
                if (!line.Contains(' '))
                    return null;

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
                return null;

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

            return new IRCMessage(rawLine, prefix, command, parameters, tags);
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
