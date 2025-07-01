using System;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using System.Threading.Tasks;
using Scitalis.TDM.MatchScore;

namespace Scitalis.TDM.Commands
{
    [Command("set_all")]
    [CommandSyntax("<score>")]
    [CommandDescription("Sets scores for all teams")]
    public class GameSetAllTeamsPointsCommand : Command
    {
        private readonly IMatchScore _matchScore;

        public GameSetAllTeamsPointsCommand(IServiceProvider serviceProvider,
            IMatchScore matchScore) : base(serviceProvider)
        {
            _matchScore = matchScore;
        }

        protected override Task OnExecuteAsync()
        {
            if (Context.Parameters.Count != 1)
                throw new UserFriendlyException("Set all teams points command has 2 parameters: " +
                                                "team name and amount of points (int)");

            int score;
            try
            {
                score = Context.Parameters.GetAsync<int>(0).Result;
            }
            catch
            {
                throw new UserFriendlyException("Points must be an integer number (positive one)");
            }

            if (score <= 0)
                throw new UserFriendlyException("Points must be a positive int");

            _matchScore.SetProgress(score);
            return Task.CompletedTask;
        }
    }
}