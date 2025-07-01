using System;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using Scitalis.TDM.MatchScore;
using Scitalis.TDM.TeamManagement;

namespace Scitalis.TDM.Commands
{
    [Command("set_team")]
    [CommandSyntax("<team name> <points>")]
    [CommandDescription("Adds point with given radius on a player position. Requires permission")]
    public class GameSetTeamPointsCommand : Command
    {
        private readonly IMatchScore _matchScore;
        private readonly ITeamDataStore _teamDataStore;

        public GameSetTeamPointsCommand(IServiceProvider serviceProvider, IMatchScore matchScore,
            ITeamDataStore teamDataStore) : base(serviceProvider)
        {
            _matchScore = matchScore;
            _teamDataStore = teamDataStore;
        }

        protected override Task OnExecuteAsync()
        {
            if (Context.Parameters.Count != 2)
                throw new UserFriendlyException("Set points command has 2 parameters: team name and amount of points (int)");

            string teamName;
            int score;
            try
            {
                teamName = Context.Parameters.GetAsync<string>(0).Result;
                score = Context.Parameters.GetAsync<int>(1).Result;
            }
            catch
            {
                throw new UserFriendlyException("Team name must be a string (existing team name) and points must be positive int");
            }

            if (score <= 0)
                throw new UserFriendlyException("Points must be a positive int");

            bool areTeamAndPointExist = _teamDataStore.Configs.Items.ContainsKey(teamName);
            if (!areTeamAndPointExist)
                throw new UserFriendlyException("There is no existing team with that name");
            
            _matchScore.SetProgress(teamName, score);
            return Task.CompletedTask;
        }
    }
}