using System;
using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using Scitalis.TDM.TeamManagement;

namespace Scitalis.TDM.Commands
{
    [Command("join")]
    [CommandDescription("Join a team and get colored prefix.")]
    [CommandSyntax("<teamName>")]
    public class TeamJoinCommand : UnturnedCommand
    {
        private readonly ITeamDataStore _teamDataStore;

        public TeamJoinCommand(ITeamDataStore teamDataStore, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _teamDataStore = teamDataStore;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var player = Context.Actor as UnturnedUser 
                         ?? throw new UserFriendlyException("Only players can use this.");

            if (Context.Parameters.Length != 1)
                throw new UserFriendlyException("Expected exactly one parameter - team name (string)");

            string teamName;
            try
            {
                teamName = Context.Parameters.GetAsync<string>(0).Result;
            }
            catch (Exception)
            {
                throw new UserFriendlyException("The team name must be specified as a string value");
            }
            
            _teamDataStore.AddUserToTeam(player.Id, teamName);
            await player.PrintMessageAsync($"You joined the team {teamName}. " +
                                     $"Your team now has {_teamDataStore.Configs.Items[teamName].PlayerIDs.Count} members");
            await _teamDataStore.SaveTeamsAsync();
        }
    }
}