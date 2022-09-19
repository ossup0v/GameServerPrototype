using Orleans;
using ServerPrototype.Actors.Grains.Messages.Requests;
using ServerPrototype.Common;

namespace ServerPrototype.Actors.Grains.Interfaces
{
    public interface IPlayerFarmGrain : IGrainWithStringKey
    {
        Task<ApiResult> StartBuildConstruction(StartBuildRequest request);
    }
}
