using System;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Commands;
using Scitalis.TDM.TeamManagement;

namespace Scitalis.TDM.Commands
{
    [Command("create")]
    [CommandDescription("Creates command with name.")]
    [CommandSyntax("<teamName>")]
    public class TeamCreateCommand : UnturnedCommand
    {
        private readonly ITeamDataStore _teamDataStore;

        public TeamCreateCommand(IServiceProvider serviceProvider, ITeamDataStore teamDataStore) : base(serviceProvider)
        {
            _teamDataStore = teamDataStore;
        }

        protected override async UniTask OnExecuteAsync()
        {
            if (Context.Parameters.Length != 1)
                throw new UserFriendlyException("Command requires exactly one string parameters: (team name)");

            string teamName;
            try
            {
                teamName = Context.Parameters.GetAsync<string>(0).Result;
            }
            catch (Exception)
            {
                throw new UserFriendlyException("The team parameter must be a string value");
            }

            _teamDataStore.AddTeam(teamName);
            await _teamDataStore.SaveTeamsAsync();
        }
    }
}