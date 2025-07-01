using System;
using OpenMod.API.Ioc;

namespace Scitalis.TDM.MatchScore
{
    [Service]
    public interface IMatchScore
    {
        void ResetProgress();
        void SetProgress(int score);
        void SetProgress(string teamName, int score);
        string GetTeamProgress(string teamName);
        event EventHandler OnTeamScoreUpdated;
    }
}