using Orleans;

namespace ServerPrototype.Interfaces.Grains
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task<ApiResult<PlayerData>> GetPlayerData(); 
        Task EndSeason();

        Task LoginNotify();
    }

    public class PlayerData 
    {
        public string Nickname { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
