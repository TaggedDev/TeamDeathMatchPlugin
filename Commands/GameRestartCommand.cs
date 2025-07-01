using System;
using System.Threading.Tasks;
using OpenMod.Core.Commands;
using Scitalis.TDM.MatchScore;

namespace Scitalis.TDM.Commands
{
    [Command("restart")]
    [CommandDescription("Resets all points and scores, does not pause the game")]
    public class GameRestartCommand : Command
    {
        private readonly IMatchScore _matchScore;

        public GameRestartCommand(IServiceProvider serviceProvider, IMatchScore matchScore) : base(serviceProvider)
        {
            _matchScore = matchScore;
        }

        protected override Task OnExecuteAsync()
        {
            _matchScore.ResetProgress();
            return Task.CompletedTask;
        }
    }
}