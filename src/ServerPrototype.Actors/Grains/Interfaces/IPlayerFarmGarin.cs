using Orleans;
using ServerPrototype.Actors.Grains.Messages.Requests;
using ServerPrototype.Common;
using ServerPrototype.Shared;

namespace ServerPrototype.Actors.Grains.Interfaces
{
    public interface IPlayerFarmGrain : IGrainWithStringKey
    {
        Task<ApiResult> StartBuildConstruction(StartBuildRequest request);
        Task<Dictionary<ResourceType, ulong>> GetResourceProducrion();
    }
}
