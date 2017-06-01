using System.Collections.Generic;
using System.Linq;
using CsIRC.Core.Events;

namespace CsIRC.Core
{
    /// <summary>
    /// Handler that takes care of IRCv3 capability negotiation and handling.
    /// </summary>
    public class CapHandler
    {
        private IRCConnection _connection;

        /// <summary>
        /// A list of all IRCv3 capabilities that the server supports.
        /// </summary>
        public List<string> AvailableCapabilities { get; private set; }

        /// <summary>
        /// A list of all IRCv3 capabilities that are currently enabled.
        /// </summary>
        public List<string> EnabledCapabilities { get; private set; }

        /// <summary>
        /// A list of capabilities that will be request when the connection to the server is established.
        /// </summary>
        public List<string> RequestedCapabilities { get; private set; }

        /// <summary>
        /// Creates a new IRCv3 capability handler.
        /// </summary>
        /// <param name="connection">The connection this handler will be used for.</param>
        public CapHandler(IRCConnection connection)
        {
            _connection = connection;
            AvailableCapabilities = new List<string>();
            EnabledCapabilities = new List<string>();

            RequestedCapabilities = new List<string>();
            RequestedCapabilities.Add("away-notify");
            RequestedCapabilities.Add("multi-prefix");
            RequestedCapabilities.Add("userhost-in-names");
        }

        /// <summary>
        /// Requests the list of capabilities that are currently enabled.
        /// </summary>
        public void RequestEnabledCapabilities()
        {
            _connection.Output.SendRaw("CAP", "LIST");
        }

        /// <summary>
        /// Requests the list of capabilities that the server supports.
        /// </summary>
        public void RequestAvailableCapabilities()
        {
            _connection.Output.SendRaw("CAP", "LS");
        }

        /// <summary>
        /// Requests a given capability to be enabled.
        /// </summary>
        /// <param name="capability">The capability that should be enabled.</param>
        public void EnableCapability(string capability)
        {
            EnableCapabilities(new List<string> { capability });
        }

        /// <summary>
        /// Requests a given list of capabilities to be enabled.
        /// </summary>
        /// <param name="capabilities">The capabilities that should be enabled.</param>
        public void EnableCapabilities(IEnumerable<string> capabilities)
        {
            RequestedCapabilities = RequestedCapabilities.Where(x => AvailableCapabilities.Contains(x)).ToList();
            _connection.Output.SendRaw("CAP", "REQ", string.Join(" ", RequestedCapabilities));
        }

        /// <summary>
        /// Checks if all requested capablities have been acknowledged or rejected and ends the capability negotiation if that is the case.
        /// </summary>
        public void CheckNegotiationCompleted()
        {
            if (!RequestedCapabilities.Any() && !_connection.IsLoggedIn)
                _connection.Output.SendRaw("CAP", "END");
        }

        /// <summary>
        /// Handle a CAP subcommand.
        /// </summary>
        /// <param name="message">The message that contains the command.</param>
        public void HandleCapReply(IRCMessage message)
        {
            switch (message.Parameters[1])
            {
                case "ACK":
                    foreach (string capability in message.Parameters[2].Split(' '))
                    {
                        if (!EnabledCapabilities.Contains(capability))
                            EnabledCapabilities.Add(capability);

                        if (RequestedCapabilities.Contains(capability))
                            RequestedCapabilities.Remove(capability);
                    }
                    CheckNegotiationCompleted();
                    break;
                case "LS":
                    AvailableCapabilities = message.Parameters[2].Split(' ').ToList();
                    EnableCapabilities(RequestedCapabilities);
                    CheckNegotiationCompleted();
                    break;
                case "NAK":
                    foreach (string capability in message.Parameters[2].Split(' '))
                        if (RequestedCapabilities.Contains(capability))
                            RequestedCapabilities.Remove(capability);
                    CheckNegotiationCompleted();
                    break;
            }
        }

        /// <summary>
        /// Try to handle an unknown command as a CAP command.
        /// </summary>
        /// <param name="message">The message that contains the command.</param>
        public void HandleCapabilityCommand(IRCMessage message)
        {
            IRCHostmask prefix = new IRCHostmask(message.Prefix);
            IRCUser user = _connection.GetUserByPrefix(prefix);
            switch (message.Command)
            {
                case "AWAY":
                    if (message.Parameters.Any())
                    {
                        user.IsAway = true;
                        user.AwayMessage = message.Parameters.First();
                    }
                    else
                    {
                        user.IsAway = false;
                        user.AwayMessage = null;
                    }
                    IRCEvents.OnUserAwayStatusChanged(this, new UserReasonCommandEventArgs(message, user, user.AwayMessage));
                    break;
            }
        }

        /// <summary>
        /// Try to handle an unknown numeric as a CAP numeric.
        /// </summary>
        /// <param name="message">The message that contains the numeric.</param>
        public void HandleCapabilityNumeric(IRCMessage message)
        {
        }
    }
}
