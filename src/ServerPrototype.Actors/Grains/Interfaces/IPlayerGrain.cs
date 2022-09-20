using Orleans;
using ServerPrototype.Actors.Grains.Messages.Requests;
using ServerPrototype.Actors.Grains.Models;
using ServerPrototype.Common;
using ServerPrototype.Common.Models;

namespace ServerPrototype.Actors.Grains.Interfaces
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task<ApiResult<PlayerInfo>> GetPlayerInfo();
        Task<ApiResult<int>> StartBuildConstruction(StartBuildRequest request);
        Task EndSeason();
        Task LoginNotify();
    }
}
