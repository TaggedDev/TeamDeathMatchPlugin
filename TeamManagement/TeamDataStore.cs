using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using Scitalis.TDM.Models;
using Steamworks;

namespace Scitalis.TDM.TeamManagement
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class TeamDataStore : ITeamDataStore
    {
        private const string ConfigsKey = "teamconfigs";

        private readonly IDataStore _dataStore;
        private readonly ILogger<TeamDeathMatchPlugin> _logger;

        public TeamConfigs Configs { get; private set; }
        public event EventHandler<string> OnTeamCreated = delegate{ };

        public TeamDataStore(IDataStore dataStore, ILogger<TeamDeathMatchPlugin> logger)
        {
            Configs = new TeamConfigs();
            _dataStore = dataStore;
            _logger = logger;
        }

        public async Task LoadTeamsAsync()
        {
            _logger.LogInformation("Loading team configs to TeamsDataStore");
            TeamConfigs? teamConfigs = await _dataStore.LoadAsync<TeamConfigs>(ConfigsKey);
            _logger.LogInformation($"Team configs read (is null={teamConfigs is null}). If true, creates new list.");
            Configs = teamConfigs ?? new TeamConfigs();
            _logger.LogInformation($"Teams loaded. Current teams: {Configs.Items.Count}");
        }

        public async Task SaveTeamsAsync()
            => await _dataStore.SaveAsync(ConfigsKey, Configs);

        public void AddUserToTeam(CSteamID steamID, string teamName)
        {
            if (!Configs.Items.TryGetValue(teamName, out var existingTeam))
            {
                string teamNames = string.Join("\n- ", Configs.Items.Keys);
                throw new UserFriendlyException($"There is no such team name \"{teamName}\". Available teams: {teamNames}");
            }

            if (existingTeam.PlayerIDs.Contains(steamID))
            {
                string teamNames = string.Join("\n- ", Configs.Items.Keys);
                throw new UserFriendlyException($"User {steamID} is already in this team. Available teams:\n -{teamNames}." +
                                                "\nPlayer can only be in one team");
            }

            Configs.HandleUserSwitchTeam(teamName, steamID);
        }

        public void RemoveUserFromTeam(CSteamID steamID)
        {
            bool wasPlayerRemoved = WasUserFoundDuringRemoving();

            if (!wasPlayerRemoved)
                throw new UserFriendlyException($"You are not in a team");
            return;

            bool WasUserFoundDuringRemoving()
            {
                foreach (TeamInfo teamInfo in Configs.Items.Values)
                {
                    wasPlayerRemoved = teamInfo.PlayerIDs.Remove(steamID);
                    if (wasPlayerRemoved)
                        return true;
                }

                return false;
            }
        }

        public bool ArePlayersTeammates(CSteamID userID1, CSteamID userID2) 
            => Configs
                .Items
                .Values
                .Any(teamInfo => teamInfo.PlayerIDs.Contains(userID1) && teamInfo.PlayerIDs.Contains(userID2));

        public void AddTeam(string teamName)
        {
            if (Configs.Items.ContainsKey(teamName))
            {
                string teamNames = string.Join("\n- ", Configs.Items.Keys);
                throw new UserFriendlyException($"Team with name {teamName} already exists. Specify another name." +
                                                $"List of existing teams:\n- {teamNames}");
            }
            
            Configs.Items.Add(teamName, new TeamInfo { TeamName = teamName, PlayerIDs = new List<CSteamID>()});
            OnTeamCreated(this, teamName);
        }
        
        public string GetTeam(CSteamID steamID)
        {
            foreach (TeamInfo teamInfo in Configs.Items.Values)
                if (teamInfo.PlayerIDs.Contains(steamID))
                    return teamInfo.TeamName;

            _logger.LogInformation($"[GetTeam]: Unturned user was not in the teamInfo");
            return string.Empty;
        }
    }
}