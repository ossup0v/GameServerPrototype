using Microsoft.Extensions.Logging;
using Orleans;
using ServerPrototype.Actors.Grains.Interfaces;
using ServerPrototype.Actors.Grains.Messages.Requests;
using ServerPrototype.Common;
using ServerPrototype.Common.Models;
using ServerPrototype.Core.Data;
using ServerPrototype.Shared;

namespace ServerPrototype.Actors.Grains
{
    public class PlayerFarmGrain : Grain<PlayerFarmGrain.PlayerFramState>, IPlayerFarmGrain
    {
        public class PlayerFramState
        {
            public List<FarmConstructionLevel> Field { get; set; } = new ();
        }
        
        //TODO research how to save Point as Dictionary key in mongo
        private Dictionary<Point, FarmConstructionLevel> _field { get; set; } = new ();
        private readonly ILogger<PlayerFarmGrain> _logger;
        public PlayerFarmGrain(ILogger<PlayerFarmGrain> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            _field = State.Field.ToDictionary(x => x.Point);
            return base.OnActivateAsync();
        }

        public async Task<ApiResult> StartBuildConstruction(StartBuildRequest request)
        {
            //TODO check is other construction is building
            if (_field.ContainsKey(request.Point))
                return ApiResult.BadRequest;

            if (!ContentProvider.Instance.GetFarmConstructions().TryGetValue(request.ConstructionId, out var construction))
                return ApiResult.BadRequest;

            if (!construction.Levels.TryGetValue(request.Level, out var level))
            {
                _logger.LogError($"Can find level {request.Level} in construction {request.ConstructionId}");
                return ApiResult.BadRequest;
            }

            level.Point = request.Point;

            await SetConstruction(level);
            _logger.LogInformation($"Construction {construction.Id} level {level.Level} builded at {level.Point}");
            //add timer here & return remain time span

            return ApiResult.OK;
        }

        private async Task SetConstruction(FarmConstructionLevel level)
        {
            State.Field.Add(level);
            _field.Add(level.Point, level);

            await WriteStateAsync();
        }

        public Task<Dictionary<ResourceType, ulong>> GetResourceProducrion()
        {
            var result = new Dictionary<ResourceType, ulong>();
            foreach (var level in _field.Values)
            {
                foreach (var resources in level.ProductionResources)
                {
                    if (!result.ContainsKey(resources.Key))
                        result.Add(resources.Key, 0UL);
                 
                    result[resources.Key] += resources.Value;
                }
            }

            return Task.FromResult(result);
        }
    }
}
