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
        
        public InputHandler Input { get; private set; }
        public OutputHandler Output { get; private set; }
        public bool IsConnected { get; internal set; }
        public Dictionary<string, IRCUser> Users { get; private set; }
        public Dictionary<string, string> UserModes { get; private set; }
        #endregion
        
        #region Constructors
        public IRCConnection()
        {
            Users = new Dictionary<string, IRCUser>();
            UserModes = new Dictionary<string, string>();
        }
        #endregion
        
        public bool Connect(string hostname, int port)
        {
            _connection = new TcpClient(hostname, port);
            _connectionStream = _connection.GetStream();
            Input = new InputHandler(new StreamReader(_connectionStream), this);
            Output = new OutputHandler(new StreamWriter(_connectionStream));

            return true;
        }

        public void Disconnect()
        {
            Output.SendQUIT();

            Input.Dispose();
            Output.Dispose();
            _connectionStream.Dispose();
            _connection.Close();
        }
    }
}
