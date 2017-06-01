using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace CsIRC.Core
{
    public class IRCConnection
    {
        #region Fields & Properties

        private TcpClient _connection;
        private NetworkStream _connectionStream;

        /// <summary>
        /// The input handler for this connection.
        /// </summary>
        public InputHandler Input { get; private set; }

        /// <summary>
        /// The output handler for this connection.
        /// </summary>
        public OutputHandler Output { get; private set; }

        /// <summary>
        /// The feature support helper for this connection.
        /// </summary>
        public SupportHandler Support { get; private set; }

        /// <summary>
        /// The IRCv3 capability helper for this connection.
        /// </summary>
        public CapHandler Capability { get; set; }

        /// <summary>
        /// Whether or not the connection is active.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Whether or not the current user is logged into the server.
        /// </summary>
        public bool IsLoggedIn { get; set; }

        /// <summary>
        /// The client's current nicname.
        /// </summary>
        public string CurrentNickname { get; set; }

        /// <summary>
        /// A list of all known users on the network that the client shares a channel with.
        /// </summary>
        public List<IRCUser> Users { get; private set; }

        /// <summary>
        /// A list of all channels the client has joined.
        /// </summary>
        public List<IRCChannel> Channels { get; private set; }

        /// <summary>
        /// The modes set on the current user.
        /// </summary>
        public Dictionary<char, object> UserModes { get; private set; }

        #endregion Fields & Properties

        #region Constructors

        /// <summary>
        /// Creates a new connection instance.
        /// </summary>
        public IRCConnection()
        {
            Users = new List<IRCUser>();
            Channels = new List<IRCChannel>();
            UserModes = new Dictionary<char, object>();
        }

        #endregion Constructors

        /// <summary>
        /// Connects to an IRC server using a hostname and a port.
        /// </summary>
        /// <param name="hostname">The IP address or hostname of the IRC server.</param>
        /// <param name="port">The port to connect on.</param>
        public void Connect(string hostname, int port)
        {
            if (IsConnected)
                Disconnect();

            _connection = new TcpClient(hostname, port);
            _connectionStream = _connection.GetStream();
            Input = new InputHandler(new StreamReader(_connectionStream), this);
            Output = new OutputHandler(new StreamWriter(_connectionStream), this);
            Support = new SupportHandler();
            Capability = new CapHandler(this);

            IsConnected = true;
        }

        /// <summary>
        /// Disconnects from the IRC server.
        /// </summary>
        public void Disconnect()
        {
            if (!IsConnected)
                return;

            Output.SendQUIT();

            Input.Dispose();
            Output.Dispose();
            _connectionStream.Dispose();
            _connection.Close();

            IsConnected = false;
        }

        /// <summary>
        /// Perform a mode change on this channel. Should only be called after intructed by the server.
        /// </summary>
        /// <param name="modeString">The modes that will be changed.</param>
        public void SetUserModes(ModeString modeString)
        {
            modeString.ApplyModeChanges(UserModes);
        }

        /// <summary>
        /// Retrieves a user object for the given nickname.
        /// </summary>
        /// <param name="nickname">The user's nickname.</param>
        /// <returns>A user or null if the user is unknown to the client.</returns>
        public IRCUser GetUserByNick(string nickname)
        {
            return Users.FirstOrDefault(x => x.Nickname == nickname);
        }

        /// <summary>
        /// Retrieves a user object for the given prefix.
        /// </summary>
        /// <param name="prefix">The user's prefix.</param>
        /// <returns>A user or null if the user is unknown to the client.</returns>
        public IRCUser GetUserByPrefix(IRCHostmask prefix)
        {
            return GetUserByNick(prefix.Nickname);
        }

        /// <summary>
        /// Retrieves a channel object for the given nickname.
        /// </summary>
        /// <param name="name">The channel's name.</param>
        /// <returns>A channel or null if the channel is unknown to the client.</returns>
        public IRCChannel GetChannelByName(string name)
        {
            return Channels.FirstOrDefault(x => x.Name == name);
        }
    }
}
