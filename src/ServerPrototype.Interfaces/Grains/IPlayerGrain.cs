using Orleans;

namespace ServerPrototype.Interfaces.Grains
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task<ApiResult<PlayerData>> GetPlayerData(); 
        Task<ApiResult> StartBuildConstruction(StartBuildRequest request);
        Task EndSeason();

        Task LoginNotify();
    }
}
