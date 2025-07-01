using System.Threading.Tasks;
using OpenMod.API.Ioc;
using Scitalis.TDM.Models;
using Steamworks;

namespace Scitalis.TDM.TeamManagement
{
    [Service]
    public interface ITeamDataStore
    {
        public TeamConfigs Configs { get; }
        Task LoadTeamsAsync();
        Task SaveTeamsAsync();
        void AddUserToTeam(CSteamID steamID, string teamName);
        void RemoveUserFromTeam(CSteamID steamID);
        void AddTeam(string teamName);
        string GetTeam(CSteamID steamID);
    }
}