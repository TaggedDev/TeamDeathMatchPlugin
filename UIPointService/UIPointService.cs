using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Connections.Events;
using OpenMod.Unturned.Users;
using Scitalis.TDM.MatchScore;
using SDG.NetTransport;
using SDG.Unturned;

namespace Scitalis.TDM.UI
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class UIPointService : IUIPointService
    {
        private readonly ILogger<TeamDeathMatchPlugin> _logger;
        private readonly IUnturnedUserDirectory _userDirectory;
        private readonly IMatchScore _matchScore;

        // HUD effect is in the .dat file
        private const ushort EffectId = 11001;
        private const short InterfaceInstanceKey = 0;
        
        public UIPointService(IUnturnedUserDirectory userDirectory, ILogger<TeamDeathMatchPlugin> logger, 
            IMatchScore matchScore)
        {
            _userDirectory = userDirectory;
            _matchScore = matchScore;
            _logger = logger;

            _matchScore.OnTeamScoreUpdated += (sender, args) =>
            {
                DisplayCurrentScore();
            };
        }

        private void ShowUI(UnturnedPlayer player)
        {
            EffectManager.sendUIEffect(EffectId, InterfaceInstanceKey, player.Player.channel.owner.transportConnection, true);
        }

        private void DisplayCurrentScore()
        {
            foreach (UnturnedUser unturnedUser in _userDirectory.GetOnlineUsers())
            {
                ITransportConnection conn = unturnedUser.Player.Player.channel.owner.transportConnection;
                EffectManager.sendUIEffectText(InterfaceInstanceKey, conn, true, "TeamA_Points",
                    $"{_matchScore.GetTeamProgress("Alpha")}");
                EffectManager.sendUIEffectText(InterfaceInstanceKey, conn, true, "TeamB_Points",
                    $"{_matchScore.GetTeamProgress("Beta")}");    
            }
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerConnectedEvent @event)
        {
            ShowUI(@event.Player);
            return Task.CompletedTask;
        }
    }
}