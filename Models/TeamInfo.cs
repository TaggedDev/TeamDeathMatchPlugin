using System;
using System.Collections.Generic;

namespace Scitalis.TDM.Models
{
    // REQUIRED: Parameterless constructor for YAML deserialization
    [Serializable]
    public class TeamInfo
    {
        public string TeamName { get; set; } = string.Empty;

        public List<string> PlayerIDs { get; set; } = new List<string>();
    }

    [Serializable]
    public class TeamConfigs
    {
        // REQUIRED: Use public property, not public field
        public Dictionary<string, TeamInfo> Items { get; set; } = new Dictionary<string, TeamInfo>();

        public void HandleUserSwitchTeam(string teamName, string userID)
        {
            foreach (var teamInfo in Items.Values)
            {
                teamInfo.PlayerIDs.Remove(userID);
            }

            if (!Items.ContainsKey(teamName))
            {
                Items[teamName] = new TeamInfo
                {
                    TeamName = teamName
                };
            }

            if (!Items[teamName].PlayerIDs.Contains(userID))
            {
                Items[teamName].PlayerIDs.Add(userID);
            }
        }

        public void RemovePlayerFromTeam(string teamName, string userID)
        {
            if (Items.TryGetValue(teamName, out var teamInfo))
            {
                teamInfo.PlayerIDs.Remove(userID);
            }
        }
    }
}