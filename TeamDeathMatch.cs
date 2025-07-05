using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.API.Persistence;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using Scitalis.TDM.Models;
using Scitalis.TDM.TeamManagement;


[assembly: PluginMetadata("Scitalis.TeamDeathMatch", DisplayName = "TeamDeathMatch")]
namespace Scitalis.TDM
{
    public class TeamDeathMatchPlugin : OpenModUnturnedPlugin
    {
        private const string ConfigsKey = "teamconfigs";
        
        private readonly ILogger<TeamDeathMatchPlugin> _logger;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IDataStore _dataStore;
        private readonly ITeamDataStore _teamDataStore;

        public TeamDeathMatchPlugin(ILogger<TeamDeathMatchPlugin> logger,
            IStringLocalizer stringLocalizer, ITeamDataStore teamDataStore, IServiceProvider serviceProvider, 
            IDataStore dataStore) : base(serviceProvider)
        {
            _stringLocalizer = stringLocalizer;
            _teamDataStore = teamDataStore;
            _dataStore = dataStore;
            _logger = logger;
        }

        protected override async UniTask OnLoadAsync()
        {
            await UniTask.SwitchToMainThread();
            await RegisterDataStores();
            await _teamDataStore.LoadTeamsAsync();
            _logger.LogInformation("[TDM] Plugin loaded");
            await UniTask.SwitchToThreadPool(); // you can switch back to a different thread
        }

        protected override async UniTask OnUnloadAsync()
        {
            await UniTask.SwitchToMainThread();
            _logger.LogInformation(_stringLocalizer["plugin_events:plugin_stop"]);
        }
        
        private async Task RegisterDataStores()
        {
            if (!await _dataStore.ExistsAsync(ConfigsKey)) 
                await _dataStore.SaveAsync(ConfigsKey, new TeamConfigs());
        }
    }
}
