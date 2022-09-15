using Orleans;
using ServerPrototype.Interfaces;
using ServerPrototype.Interfaces.Grains;

namespace ServerPrototype.Actors.Grains
{
    public class PlayerGrain : Grain, IPlayerGrain
    {
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
    }
}
