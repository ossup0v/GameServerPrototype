using Orleans;

namespace ServerPrototype.Interfaces.Grains
{
    public interface IPlayerFarmGrain : IGrainWithStringKey
    {
        Task<ApiResult> StartBuildConstruction(StartBuildRequest request);
    }
}
