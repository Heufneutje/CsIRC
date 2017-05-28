using System.Collections.Generic;
using System.Linq;
using CsIRC.Utils;

namespace CsIRC.Core
{
    public class ModeString
    {
        public List<ModeChange> ModesChanged { get; private set; }

        public ModeString()
        {
            ModesChanged = new List<ModeChange>();
        }

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

        public List<ModeString> Split(int maxModes)
        {
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
            return $"{modes} {string.Join(" ", parameters)}";
        }
    }
}
