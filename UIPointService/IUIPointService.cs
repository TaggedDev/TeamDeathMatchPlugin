using System.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Players.Connections.Events;
using OpenMod.Unturned.Plugins;

namespace Scitalis.TDM.UI
{
    [Service]
    public interface IUIPointService : IEventListener<UnturnedPlayerConnectedEvent>
    {
        Task LoadAsync(OpenModUnturnedPlugin plugin);
    }
}