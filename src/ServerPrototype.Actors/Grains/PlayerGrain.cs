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
            public bool IsNotFirstLogin { get; set; }
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

        protected override async Task Init()
        {
            await AddComponent(new InventoryComponent(_serviceProvider.GetRequiredService<IPlayerInventoryDb>()), State.Id);
            await AddComponent(new EffectContainerComponent(_serviceProvider.GetRequiredService<IPlayerEffectsDb>()), State.Id);

            await InitFirstLogin();

            State.PlayerData.LastLogin = DateTime.UtcNow;

            await WriteStateAsync();
        }

        private async Task InitFirstLogin()
        {
            if (State.IsNotFirstLogin)
                return;

            State.LastResourceIncomeTime ??= DateTime.UtcNow; //for first login
            State.PlayerData ??= new PlayerData
            {
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Nickname = String.Empty
            };

            State.Id ??= GrainReference.GetPrimaryKeyString();

            var inventory = GetComponent<InventoryComponent>();

            foreach (var resource in await ContentProvider.Instance.GetFirstLoginResources())
                inventory.IncreaseResource(resource.Key, resource.Value);

            await inventory.SaveChanges();
            await WriteStateAsync();

            State.IsNotFirstLogin = true;
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
            //get find construction from static info
            if (!ContentProvider.Instance.GetFarmConstructions().TryGetValue(request.ConstructionId, out var construction))
                return ApiResult.BadRequest;

            if (!construction.Levels.TryGetValue(request.Level, out var level))
            {
                _logger.LogError($"Can find level {request.Level} in construction {request.ConstructionId}");
                return ApiResult.BadRequest;
            }
            //check resources enough
            if (!CheckResourceRequirements(level.ResourceRequirements))
                return ApiResult.InternalError;

            //recalculate resources, add rss before bonus start work
            await AddResources();

            //find farm grain
            var farmGrain = GrainFactory.GetGrain<IPlayerFarmGrain>(GetPlayerFarmActorId());
            var result = await farmGrain.StartBuildConstruction(request);

            if (result.IsFailed)
                return result;

            // TODO move it in BuildConstructionComplited
            await AddEffects(level.Effects);

            var decreaseResult = await TryDescreaseResources(level.ResourceToSpend);
            if (decreaseResult is not true)
            {
                _logger.LogError("StartBuildConstruction check requirements passed, but can't decrease resources");
                return ApiResult.InternalError;
            }

            return result;
        }

        private async Task AddResources()
        {
            var inventory = GetComponent<InventoryComponent>();

            _logger.LogDebug("before add resources to player {@player_id} {@resources}", State.Id, inventory.Resources);

            var resourcesToAdd = ResourceProductionCalculator.GetResourceAmountToReceive(
                State.LastResourceIncomeTime.Value,
                await GetResourcesProductionBase(),
                GetResourcesProductionBoosts(),
                _logger);


            foreach (var resToAdd in resourcesToAdd)
                inventory.IncreaseResource(resToAdd.Key, resToAdd.Value);

            _logger.LogDebug("after add resources to player {@player_id} {@resources}", State.Id, inventory.Resources);

            State.LastResourceIncomeTime = DateTime.UtcNow;
            await WriteStateAsync();
            await inventory.SaveChanges();
        }

        private async Task<bool> TryDescreaseResources(Dictionary<ResourceType, ulong> resources)
        {
            await AddResources();
            var inventory = GetComponent<InventoryComponent>();
            _logger.LogDebug("before decrease resources from player {@player_id} {@resources}", State.Id, inventory.Resources);


            if (inventory.TryDescreaseResource(resources) is not true)
                return false;

            _logger.LogDebug("after decrease resources from player {@player_id} {@resources}", State.Id, inventory.Resources);

            State.LastResourceIncomeTime = DateTime.UtcNow;
            await WriteStateAsync();
            await inventory.SaveChanges();

            return true;
        }

        private async Task AddEffects(List<Effect> effects)
        {
            var effectsContainer = GetComponent<EffectContainerComponent>();

            foreach (var effect in effects)
                effectsContainer.AddEffect(effect);

            await effectsContainer.SaveChanges();
        }

        private Task<Dictionary<ResourceType, ulong>> GetResourcesProductionBase()
        {
            var farmGrain = GrainFactory.GetGrain<IPlayerFarmGrain>(GetPlayerFarmActorId());

            return farmGrain.GetResourceProducrion();
        }

        private Dictionary<ResourceType, double> GetResourcesProductionBoosts()
        {
            var effects = GetComponent<EffectContainerComponent>();
            return effects.GetAggregatedResourceProductionEffects();
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
                if (!inventory.CheckResourceIsEnough(requirement.Resource, requirement.RequirementAmountOfResource))
                    return false;
            }

            return true;
        }
    }
}
