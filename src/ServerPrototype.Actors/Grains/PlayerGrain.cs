using Microsoft.Extensions.Logging;
using Orleans;
using ServerPrototype.Interfaces;
using ServerPrototype.Interfaces.Grains;
using ServerPrototype.Shared;

namespace ServerPrototype.Actors.Grains
{
    public class PlayerGrain : Grain<PlayerGrain.PlayerGrainState>, IPlayerGrain
    {
        public class PlayerGrainState
        {
            public string Id { get; set; }
            public DateTime? LastResourceIncomeTime { get; set; }
            public Dictionary<int, ulong> Resources { get; set; } = new Dictionary<int, ulong>();
        }
        
        private readonly ILogger<PlayerGrain> _logger;

        public PlayerGrain(ILogger<PlayerGrain> logger)
        {
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            State.LastResourceIncomeTime ??= DateTime.UtcNow; //for first login
            State.Id = GrainReference.GetPrimaryKeyString();

            return base.OnActivateAsync();
        }


        public Task EndSeason()
        {
            return Task.CompletedTask;
        }

        public Task<ApiResult<PlayerData>> GetPlayerData()
        {
            return Task.FromResult(ApiResult<PlayerData>.OK(new PlayerData()));
        }

        public Task LoginNotify()
        {
            return Task.CompletedTask;
        }

        public async Task<ApiResult> StartBuildConstruction(StartBuildRequest request)
        {
            _logger.LogInformation("Trying to start build construction in farm grain, request {@request}", request);
            //TODO
            //get find construction from static info
            //get requered rss to build
            //check that's enough

            //recalculate resources, add rss before bonus start work
            await AddResources();

            //find farm grain
            var farmGrain = GrainFactory.GetGrain<IPlayerFarmGrain>(GetPlayerFarmActorId());
            return await farmGrain.StartBuildConstruction(request);
        }

        private async Task AddResources()
        {
            _logger.LogDebug("before add resources to player {@player_id} {@resources}", State.Id, State.Resources);
            var resourcesToAdd = ResourceProfitCalculator.GetResourceAmountToReceive(
                State.LastResourceIncomeTime.Value,
                await GetResourcesMinigPerSec(),
                await GetResourcesMinigBoosts(),
                _logger);

            foreach (var resToAdd in resourcesToAdd)
            {
                if (!State.Resources.ContainsKey(resToAdd.Key))
                    State.Resources.Add(resToAdd.Key, 0UL);

                State.Resources[resToAdd.Key] += resToAdd.Value;
            }

            _logger.LogDebug("after add resources to player {@player_id} {@resources}", State.Id, State.Resources);
            State.LastResourceIncomeTime = DateTime.UtcNow;
            await WriteStateAsync();
        }

        
        private Task<Dictionary<int, ulong>> GetResourcesMinigPerSec()
        {
            return Task.FromResult(new Dictionary<int, ulong>
            {
                [1] = 100,
                [2] = 200,
                [3] = 300,
            });
        }

        private Task<Dictionary<int, decimal>> GetResourcesMinigBoosts()
        {
            return Task.FromResult(new Dictionary<int, decimal>
            {
                [1] = 0.3M,
                [2] = 0.6M,
                [3] = 0.9M,
            });
        }

        private string GetPlayerFarmActorId()
        {
            return $"{IdentityString}_farm_1";
        }
    }
}
