using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using ServerPrototype.Actors.Grains.Common.Components;
using ServerPrototype.Actors.Grains.Interfaces;
using ServerPrototype.Actors.Grains.Messages.Requests;
using ServerPrototype.Actors.Grains.Models;
using ServerPrototype.Common;
using ServerPrototype.Common.Models;
using ServerPrototype.Core.Components;
using ServerPrototype.Core.Data;
using ServerPrototype.DAL.Api;
using ServerPrototype.Shared;

namespace ServerPrototype.Actors.Grains
{
    public class PlayerGrain : ComponentedGrainBase<PlayerGrain.PlayerGrainState>, IPlayerGrain
    {
        public class PlayerGrainState
        {
            public string Id { get; set; }
            public DateTime? LastResourceIncomeTime { get; set; }
            public PlayerData PlayerData { get; set; }
        }
        
        private readonly ILogger<PlayerGrain> _logger;
        private readonly IServiceProvider _serviceProvider;

        public PlayerGrain(ILogger<PlayerGrain> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override Task Init()
        {
            State.LastResourceIncomeTime ??= DateTime.UtcNow; //for first login
            State.PlayerData ??= new PlayerData
            {
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Nickname = String.Empty
            };

            State.Id ??= GrainReference.GetPrimaryKeyString();

            AddComponent(new InventoryComponent(_serviceProvider.GetRequiredService<IPlayerInventoryDb>()), State.Id);
            AddComponent(new EffectContainerComponent(_serviceProvider.GetRequiredService<IPlayerEffectsDb>()), State.Id);

            return Task.CompletedTask;
        }

        public Task EndSeason()
        {
            return Task.CompletedTask;
        }

        public Task<ApiResult<PlayerData>> GetPlayerData()
        {
            return Task.FromResult(ApiResult<PlayerData>.OK(State.PlayerData));
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
            if (!ContentProvider.Instance.GetFarmConstructions().TryGetValue(request.ConstructionId, out var construction))
                return ApiResult.BadRequest;
            
            //check resources enough
            //todo check is other construction is building
            var firstLevel = construction.Levels[construction.Levels.Keys.Min()];
            if (!CheckResourceRequirements(firstLevel.ResourceRequirements))
                return ApiResult.InternalError;

            //recalculate resources, add rss before bonus start work
            await AddResources();

            //find farm grain
            var farmGrain = GrainFactory.GetGrain<IPlayerFarmGrain>(GetPlayerFarmActorId());
            var result = await farmGrain.StartBuildConstruction(request);

            if (result.IsFailed)
                return result;

            var effects = GetComponent<EffectContainerComponent>();

            foreach (var effect in firstLevel.Effects)
                effects.AddEffect(effect);

            await effects.SaveChanges();

            return result;
        }

        private async Task AddResources()
        {
            var inventory = GetComponent<InventoryComponent>();
            _logger.LogDebug("before add resources to player {@player_id} {@resources}", State.Id, inventory.Resources);
            var resourcesToAdd = ResourceProfitCalculator.GetResourceAmountToReceive(
                State.LastResourceIncomeTime.Value,
                await GetResourcesMinigBasePerSec(),
                await GetResourcesMinigBoosts(),
                _logger);

            
            foreach (var resToAdd in resourcesToAdd)
                inventory.IncreaseResource(resToAdd.Key, resToAdd.Value);
            

            _logger.LogDebug("after add resources to player {@player_id} {@resources}", State.Id, inventory.Resources);

            State.LastResourceIncomeTime = DateTime.UtcNow;
            await WriteStateAsync();
            await inventory.SaveChanges();
        }

        ///TODO get it from <see cref="IPlayerFarmGrain"/>
        private Task<Dictionary<int, ulong>> GetResourcesMinigBasePerSec()
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
            return $"{State.Id}_farm_1";
        }

#warning TODO rewrite to base requirement!
        private bool CheckResourceRequirements(List<ResourceRequirement> requirements)
        {
            var inventory = GetComponent<InventoryComponent>();
            foreach (var requirement in requirements)
            {
                if(!inventory.CheckIsEnough(requirement.ResourceId, requirement.RequirementAmountOfResource))
                    return false;
            }

            return true;
        }
    }
}
