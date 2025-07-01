using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Users;
using Scitalis.TDM.Models;

namespace Scitalis.TDM.TeamManagement
{
    [Service]
    public interface ITeamDataStore
    {
        public TeamConfigs Configs { get; }
        Task LoadTeamsAsync();
        Task SaveTeamsAsync();
        void AddUserToTeam(string userID, string teamName);
        void RemoveUserFromTeam(string userID);
        bool ArePlayersTeammates(string userID1, string userID2);
        void AddTeam(string teamName);
        string GetTeam(UnturnedUser? unturnedUser);
    }
}