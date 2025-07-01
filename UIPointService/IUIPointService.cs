using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Players.Connections.Events;

namespace Scitalis.TDM.UI
{
    [Service]
    public interface IUIPointService : IEventListener<UnturnedPlayerConnectedEvent>
    {
        
    }
}