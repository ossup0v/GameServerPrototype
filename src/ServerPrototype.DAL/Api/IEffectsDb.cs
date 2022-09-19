using ServerPrototype.Common.Models;

namespace ServerPrototype.DAL.Api
{
    public interface IEffectsDb
    {
        Task<Dictionary<EffectKind, Effect>> GetEffects(string owner);
        Task SaveEffects(Dictionary<EffectKind, Effect> effects, string owner);
    }

    public interface IPlayerEffectsDb : IEffectsDb
    {

    }
}
