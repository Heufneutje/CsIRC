using System.Collections.Generic;
using System.Linq;
using CsIRC.Utils;

namespace CsIRC.Core
{
    /// <summary>
    /// Handler to keep track of all the features the server supports.
    /// </summary>
    public class SupportHandler
    {
        /// <summary>
        /// The hostname/IP address of the server.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// The IRC daemon and its version that the server is running.
        /// </summary>
        public string ServerVersion { get; set; }

        /// <summary>
        /// The name of the IRC network the server is a part of.
        /// </summary>
        public string NetworkName { get; set; }

        /// <summary>
        /// All ISUPPORT tokens received from the server.
        /// </summary>
        public Dictionary<string, string> RawTokens { get; private set; }

        /// <summary>
        /// All supported channel modes and their types.
        /// </summary>
        public Dictionary<char, ModeType> ChannelModes { get; private set; }

        /// <summary>
        /// All supported user modes and their types.
        /// </summary>
        public Dictionary<char, ModeType> UserModes { get; private set; }

        /// <summary>
        /// All supported status modes and the symbols that represent them.
        /// </summary>
        public OrderedDictionary<char, char> StatusModes { get; private set; }

        /// <summary>
        /// All supported status symbols and the modes that are associated with them.
        /// </summary>
        public OrderedDictionary<char, char> StatusSymbols { get; private set; }

        /// <summary>
        /// The supported prefix characters for channels.
        /// </summary>
        public List<char> ChannelTypes { get; private set; }

        /// <summary>
        /// Creates a new handler and initializes it with the basic features defined in RFC1459.
        /// </summary>
        public SupportHandler()
        {
            RawTokens = new Dictionary<string, string>();
            ChannelModes = new Dictionary<char, ModeType>()
            {
                { 'b', ModeType.List },
                { 'k', ModeType.ParamUnset },
                { 'l', ModeType.ParamSet },
                { 'm', ModeType.NoParam },
                { 'n', ModeType.NoParam },
                { 'p', ModeType.NoParam },
                { 's', ModeType.NoParam },
                { 't', ModeType.NoParam }
            };
            UserModes = new Dictionary<char, ModeType>()
            {
                { 'i', ModeType.NoParam },
                { 'o', ModeType.NoParam },
                { 's', ModeType.ParamSet },
                { 'w', ModeType.NoParam }
            };
            StatusModes = new OrderedDictionary<char, char>()
            {
                { 'o', '@' },
                { 'v', '+' }
            };
            StatusSymbols = new OrderedDictionary<char, char>()
            {
                {'@', 'o' },
                {'+', 'v' }
            };
            ChannelTypes = new List<char>() { '#' };
        }

        public void ParseTokens(List<string> tokens)
        {
            for (int tokenIndex = 1; tokenIndex < tokens.Count - 1; tokenIndex++)
            {
                string token = tokens[tokenIndex];
                string tokenKey;
                string tokenValue = null;

                if (token.Contains('='))
                {
                    string[] tokenSplit = token.Split('=');
                    tokenKey = tokenSplit[0].ToUpper();
                    tokenValue = tokenSplit[1];
                }
                else
                    tokenKey = token.ToUpper();

                RawTokens.Add(tokenKey, tokenValue);

                switch (tokenKey)
                {
                    case "CHANTYPES":
                        ChannelTypes.Clear();
                        foreach (char chanType in tokenValue)
                            ChannelTypes.Add(chanType);
                        break;
                    case "CHANMODES":
                        ParseModeGroups(tokenValue, ChannelModes);
                        break;
                    case "NETWORK":
                        NetworkName = tokenValue;
                        break;
                    case "PREFIX":
                        StatusModes.Clear();
                        StatusSymbols.Clear();
                        int parIndex = tokenValue.IndexOf(')');
                        string modes = tokenValue.Substring(1, parIndex - 1);
                        string symbols = tokenValue.Substring(parIndex + 1);
                        for (int statusIndex = 0; statusIndex < modes.Length; statusIndex++)
                        {
                            StatusModes.Add(modes[statusIndex], symbols[statusIndex]);
                            StatusSymbols.Add(symbols[statusIndex], modes[statusIndex]);
                        }
                        break;
                    case "USERMODES":
                        ParseModeGroups(tokenValue, UserModes);
                        break;
                }
            }
        }

        private void ParseModeGroups(string tokenValue, Dictionary<char, ModeType> modesDictionary)
        {
            modesDictionary.Clear();
            string[] modeGroups = tokenValue.Split(',');
            for (int modeIndex = 0; modeIndex < 4; modeIndex++)
                foreach (char modeChar in modeGroups[modeIndex])
                    modesDictionary.Add(modeChar, (ModeType)modeIndex);
        }
    }
}
