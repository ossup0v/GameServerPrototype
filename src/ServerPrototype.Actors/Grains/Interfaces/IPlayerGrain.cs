using Orleans;
using ServerPrototype.Actors.Grains.Messages.Requests;
using ServerPrototype.Actors.Grains.Models;
using ServerPrototype.Common;

namespace ServerPrototype.Actors.Grains.Interfaces
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task<ApiResult<PlayerData>> GetPlayerData();
        Task<ApiResult> StartBuildConstruction(StartBuildRequest request);
        Task EndSeason();
        Task LoginNotify();
    }
}
