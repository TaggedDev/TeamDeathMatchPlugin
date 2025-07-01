using System;
using System.Text;
using System.Threading.Tasks;
using OpenMod.Core.Commands;
using Scitalis.TDM.Models;
using Scitalis.TDM.TeamManagement;

namespace Scitalis.TDM.Commands
{
    [Command("teams")]
    [CommandDescription("All registered teams")]
    public class TeamShowCommand : Command
    {
        private readonly ITeamDataStore _teamDataStore;

        public TeamShowCommand(IServiceProvider serviceProvider, ITeamDataStore teamDataStore) : base(serviceProvider)
        {
            _teamDataStore = teamDataStore;
        }

        protected override Task OnExecuteAsync()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Registered teams:");
            foreach (TeamInfo teamInfo in _teamDataStore.Configs.Items.Values) 
                builder.AppendLine($"{teamInfo.TeamName} - {teamInfo.PlayerIDs.Count}");
            Context.Actor.PrintMessageAsync(builder.ToString());
            return Task.CompletedTask;
        }
    }
}