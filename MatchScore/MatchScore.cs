using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Players.Life.Events;
using OpenMod.Unturned.Users;
using Scitalis.TDM.TeamManagement;
using SDG.Unturned;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Scitalis.TDM.MatchScore
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class MatchScore : IMatchScore, IEventListener<UnturnedPlayerDeathEvent>
    {
        private readonly ITeamDataStore _teamData;
        private readonly IUnturnedUserDirectory _userDirectory;

        public MatchScore(ITeamDataStore teamData, IUnturnedUserDirectory userDirectory)
        {
            _teamData = teamData;
            _userDirectory = userDirectory;
        }

        public void ResetProgress()
        {
            throw new System.NotImplementedException();
        }

        public void SetProgress(int score)
        {
            throw new System.NotImplementedException();
        }

        public void SetProgress(string teamName, int score)
        {
            throw new System.NotImplementedException();
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerDeathEvent @event)
        {
            // Victim's name
            UnturnedUser? victimUser = _userDirectory.FindUser(@event.Player.SteamId);
            if (victimUser == null)
                return Task.CompletedTask;

            UnturnedUser? killerUser = _userDirectory.FindUser(@event.Instigator) ?? victimUser;

            Vector3 victimPosition = victimUser.Player.Transform.Position;
            float distance = Vector3.Distance(victimPosition, killerUser.Player.Transform.Position);


            // Default values in case there is no killer or weapon
            string killerName = killerUser?.DisplayName ?? victimUser.DisplayName;
            string weapon = @event.Player.DamageSourceName;

            string message = $"Player {victimUser.DisplayName} was killed by " +
                       $"{killerName} using {weapon} ({distance}m)";
            ChatManager.serverSendMessage(message, Color.gray);
            return Task.CompletedTask;
        }
    }
}