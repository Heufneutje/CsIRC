using System.Collections.Generic;
using System.Linq;
using CsIRC.Utils;

namespace CsIRC.Core
{
    /// <summary>
    /// A representation of different mode change combined in a single string.
    /// </summary>
    public class ModeString
    {
        /// <summary>
        /// The modes that have been added and removed.
        /// </summary>
        public List<ModeChange> ModesChanged { get; private set; }

        /// <summary>
        /// Creates a blank mode string.
        /// </summary>
        public ModeString()
        {
            ModesChanged = new List<ModeChange>();
        }

        /// <summary>
        /// Creates a mode string from modes and the parameters they take.
        /// </summary>
        /// <param name="modes">The modes that are to be changed.</param>
        /// <param name="parameters">The parameters that the modes take.</param>
        /// <param name="supportedModes">The modes that the server supports depending on the target.</param>
        /// <param name="supportedStatuses">The channel statuses that the server supports.</param>
        public ModeString(string modes, List<string> parameters, Dictionary<char, ModeType> supportedModes, List<char> supportedStatuses)
        {
            ModesChanged = new List<ModeChange>();

            bool isAdding = false;
            foreach (char mode in modes)
            {
                string param = null;

                if (mode == '+')
                {
                    isAdding = true;
                    continue;
                }
                else if (mode == '-')
                {
                    isAdding = false;
                    continue;
                }
                else if (!(supportedModes?.ContainsKey(mode)).GetValueOrDefault(false) && !(supportedStatuses?.Contains(mode)).GetValueOrDefault(false))
                    continue;

                ModeType modeType;
                if (supportedModes.ContainsKey(mode))
                    modeType = supportedModes[mode];
                else
                    modeType = ModeType.Status;

                switch (modeType)
                {
                    case ModeType.List:
                    case ModeType.ParamUnset:
                    case ModeType.Status:
                        param = parameters.Pop(0);
                        break;
                    case ModeType.ParamSet:
                        if (isAdding)
                            param = parameters.Pop(0);
                        break;
                }

                ModesChanged.Add(new ModeChange(mode, param, isAdding, modeType));
            }
        }

        /// <summary>
        /// Split the mode string into multiple mode strings based on the maximum number of changes that's allowed within a single message.
        /// </summary>
        /// <param name="maxModes">The maximum number of mode changes as provided by the server.</param>
        /// <returns>A list of mode strings splitted by the maximum number of mode changes.</returns>
        public List<ModeString> Split(int maxModes)
        {
            if (maxModes <= 0)
                return new List<ModeString>() { this };

            List<ModeString> modeStrings = new List<ModeString>();
            ModeString current = null;
            foreach (ModeChange change in ModesChanged)
            {
                if (!modeStrings.Any() || current.ModesChanged.Count == maxModes)
                {
                    ModeString newModeString = new ModeString();
                    current = newModeString;
                    modeStrings.Add(newModeString);
                }
                current.ModesChanged.Add(change);
            }
            return modeStrings;
        }

        /// <summary>
        /// Apply the mode changes in this mode string to a given dictionary of modes.
        /// </summary>
        /// <param name="currentModes">The given dictionary of user or channel modes.</param>
        public void ApplyModeChanges(Dictionary<char, object> currentModes)
        {
            foreach (ModeChange change in ModesChanged.Where(x => x.ModeType != ModeType.Status))
            {
                if (change.ModeType == ModeType.List)
                {
                    if (change.IsAdding)
                    {
                        if (!currentModes.ContainsKey(change.Mode))
                            currentModes.Add(change.Mode, new List<string>());
                        ((List<string>)currentModes[change.Mode]).Add(change.Parameter);
                    }
                    else
                    {
                        if (currentModes.ContainsKey(change.Mode) && ((List<string>)currentModes[change.Mode]).Contains(change.Parameter))
                            ((List<string>)currentModes[change.Mode]).Remove(change.Parameter);
                    }
                }
                else if (change.ModeType != ModeType.Status)
                {
                    if (change.IsAdding)
                    {
                        if (currentModes.ContainsKey(change.Mode))
                            currentModes[change.Mode] = change.Parameter;
                        else
                            currentModes.Add(change.Mode, change.Parameter);
                    }
                    else if (currentModes.ContainsKey(change.Mode))
                        currentModes.Remove(change.Mode);
                }
            }
        }

        /// <summary>
        /// Apply the status changes in this mode string to a given dictionary of channel users.
        /// </summary>
        /// <param name="currentStatuses">The given dictionary of channel users.</param>
        public void ApplyStatusChanges(Dictionary<IRCUser, List<char>> currentStatuses)
        {
            foreach (ModeChange change in ModesChanged.Where(x => x.ModeType == ModeType.Status))
            {
                IRCUser user = currentStatuses.Keys.FirstOrDefault(x => x.Nickname == change.Parameter);
                if (user == null)
                    continue;

                if (change.IsAdding && !currentStatuses[user].Contains(change.Mode))
                    currentStatuses[user].Add(change.Mode);
                else if (!change.IsAdding && currentStatuses[user].Contains(change.Mode))
                    currentStatuses[user].Remove(change.Mode);
            }
        }

        /// <summary>
        /// Override for string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            bool isAdding = false;
            string modes = string.Empty;
            List<string> parameters = new List<string>();

            foreach (ModeChange change in ModesChanged)
            {
                if (!isAdding && change.IsAdding)
                {
                    isAdding = true;
                    modes += '+';
                }
                else if (isAdding && !change.IsAdding)
                {
                    isAdding = false;
                    modes += '-';
                }
                modes += change.Mode;
                if (change.Parameter != null)
                    parameters.Add(change.Parameter);
            }
            return $"{modes} {string.Join(" ", parameters)}".TrimEnd();
        }
    }
}
