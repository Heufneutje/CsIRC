using System.Collections.Generic;
using System.IO;
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
        public Dictionary<string, object> UserModes { get; private set; }
        #endregion
        
        #region Constructors
        /// <summary>
        /// Creates a new connection instance.
        /// </summary>
        public IRCConnection()
        {
            Users = new List<IRCUser>();
            Channels = new List<IRCChannel>();
            UserModes = new Dictionary<string, object>();
        }
        #endregion
        
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
    }
}
