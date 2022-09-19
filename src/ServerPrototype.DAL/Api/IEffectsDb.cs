using ServerPrototype.Common.Models;

namespace ServerPrototype.DAL.Api
{
    public interface IEffectsDb
    {
        Task<Dictionary<EffectType, Effect>> GetEffects(string owner);
        Task SaveEffects(Dictionary<EffectType, Effect> effects, string owner);
    }

    public interface IPlayerEffectsDb : IEffectsDb
    {

    }
}
