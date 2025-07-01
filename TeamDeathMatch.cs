using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using Scitalis.TDM.MatchScore;
using Scitalis.TDM.TeamManagement;
using Scitalis.TDM.UI;


[assembly: PluginMetadata("Scitalis.TeamDeathMatch", DisplayName = "TeamDeathMatch")]
namespace Scitalis.TDM
{
    public class TeamDeathMatchPlugin : OpenModUnturnedPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TeamDeathMatchPlugin> _logger;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IUIPointService _uiPointService;
        private readonly ITeamDataStore _teamDataStore;
        private readonly IMatchScore _matchScore;

        public TeamDeathMatchPlugin(
            IConfiguration configuration,
            ILogger<TeamDeathMatchPlugin> logger,
            IStringLocalizer stringLocalizer,
            IUIPointService uiPointService,
            ITeamDataStore teamDataStore,
            IMatchScore matchScore,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _uiPointService = uiPointService;
            _teamDataStore = teamDataStore;
            _matchScore = matchScore;
        }

        protected override async UniTask OnLoadAsync()
        {
            await UniTask.SwitchToMainThread();
            await _teamDataStore.LoadTeamsAsync();
            _logger.LogInformation("[TDM] Plugin loaded");
            await UniTask.SwitchToThreadPool(); // you can switch back to a different thread
        }

        protected override async UniTask OnUnloadAsync()
        {
            await UniTask.SwitchToMainThread();
            _logger.LogInformation(_stringLocalizer["plugin_events:plugin_stop"]);
        }
    }
}
