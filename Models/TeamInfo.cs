using System;
using System.Collections.Generic;
using Steamworks;

namespace Scitalis.TDM.Models
{
    // REQUIRED: Parameterless constructor for YAML deserialization
    [Serializable]
    public class TeamInfo
    {
        public string TeamName { get; set; } = string.Empty;

        public List<CSteamID> PlayerIDs { get; set; } = new List<CSteamID>();
    }

    [Serializable]
    public class TeamConfigs
    {
        // REQUIRED: Use public property, not public field
        public Dictionary<string, TeamInfo> Items { get; set; } = new Dictionary<string, TeamInfo>();

        public void HandleUserSwitchTeam(string teamName, CSteamID steamID)
        {
            foreach (var teamInfo in Items.Values)
            {
                teamInfo.PlayerIDs.Remove(steamID);
            }

            if (!Items.ContainsKey(teamName))
            {
                Items[teamName] = new TeamInfo
                {
                    TeamName = teamName
                };
            }

            if (!Items[teamName].PlayerIDs.Contains(steamID))
            {
                Items[teamName].PlayerIDs.Add(steamID);
            }
        }

        public void RemovePlayerFromTeam(string teamName, CSteamID steamID)
        {
            if (Items.TryGetValue(teamName, out var teamInfo)) 
                teamInfo.PlayerIDs.Remove(steamID);
        }
    }
}