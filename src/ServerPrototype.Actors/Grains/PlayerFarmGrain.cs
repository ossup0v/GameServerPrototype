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
            public List<FarmConstruction> Field { get; set; } = new ();
        }
        
        private Dictionary<Point, FarmConstruction> _field { get; set; } = new ();

        public override Task OnActivateAsync()
        {
            _field = State.Field.ToDictionary(x => x.Point);
            return base.OnActivateAsync();
        }

        public async Task<ApiResult> StartBuildConstruction(StartBuildRequest request)
        {
            if (_field.ContainsKey(request.Point))
                return ApiResult.BadRequest;

            if (!ContentProvider.Instance.GetFarmConstructions().TryGetValue(request.ConstructionId, out var construction))
                return ApiResult.BadRequest;

            construction.Point = request.Point;
            
            await SetConstruction(construction);
            //add timer here & return remain time span

            return ApiResult.OK;
        }

        private async Task SetConstruction(FarmConstruction farmConstruction)
        {
            State.Field.Add(farmConstruction);
            _field.Add(farmConstruction.Point, farmConstruction);

            await WriteStateAsync();
        }
    }
}
