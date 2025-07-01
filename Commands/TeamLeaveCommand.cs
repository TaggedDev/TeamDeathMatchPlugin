using System;
using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using Scitalis.TDM.TeamManagement;

namespace Scitalis.TDM.Commands
{
    [Command("leave")]
    [CommandDescription("Leaves current team")]
    public class TeamLeaveCommand : UnturnedCommand
    {
        private readonly ITeamDataStore _teamDataStore;

        public TeamLeaveCommand(IServiceProvider serviceProvider, ITeamDataStore teamDataStore) : base(serviceProvider)
        {
            _teamDataStore = teamDataStore;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var player = Context.Actor as UnturnedUser 
                         ?? throw new UserFriendlyException("Only players can use this.");
            _teamDataStore.RemoveUserFromTeam(player.Id);
            await _teamDataStore.SaveTeamsAsync();
        }
    }
}