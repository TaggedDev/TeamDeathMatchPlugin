using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.Unturned.Users;
using Scitalis.TDM.Models;

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

        public void AddUserToTeam(string userID, string teamName)
        {
            if (!Configs.Items.TryGetValue(teamName, out var existingTeam))
            {
                string teamNames = string.Join("\n- ", Configs.Items.Keys);
                throw new UserFriendlyException($"There is no such team name \"{teamName}\". Available teams: {teamNames}");
            }

            if (existingTeam.PlayerIDs.Contains(userID))
            {
                string teamNames = string.Join("\n- ", Configs.Items.Keys);
                throw new UserFriendlyException($"User {userID} is already in this team. Available teams:\n -{teamNames}." +
                                                "\nPlayer can only be in one team");
            }

            Configs.HandleUserSwitchTeam(teamName, userID);
        }

        public void RemoveUserFromTeam(string userID)
        {
            bool wasPlayerRemoved = WasUserFoundDuringRemoving();

            if (!wasPlayerRemoved)
                throw new UserFriendlyException($"You are not in a team");
            return;

            bool WasUserFoundDuringRemoving()
            {
                foreach (TeamInfo teamInfo in Configs.Items.Values)
                {
                    wasPlayerRemoved = teamInfo.PlayerIDs.Remove(userID);
                    if (wasPlayerRemoved)
                        return true;
                }

                return false;
            }
        }

        public bool ArePlayersTeammates(string userID1, string userID2) 
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
            
            Configs.Items.Add(teamName, new TeamInfo { TeamName = teamName, PlayerIDs = new List<string>()});
            OnTeamCreated(this, teamName);
        }

        public string GetTeam(UnturnedUser? unturnedUser)
        {
            if (unturnedUser == null)
            {
                _logger.LogInformation($"[GetTeam]: Unturned user was null");
                return string.Empty;
            }
            
            foreach (TeamInfo teamInfo in Configs.Items.Values)
                if (teamInfo.PlayerIDs.Contains(unturnedUser.Id))
                    return teamInfo.TeamName;

            _logger.LogInformation($"[GetTeam]: Unturned user was not in the teamInfo");
            return string.Empty;
        }
    }
}