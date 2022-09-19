using ServerPrototype.Common.Models;
using ServerPrototype.DAL.Api;

namespace ServerPrototype.Core.Components
{
    public class EffectContainerComponent : ComponentBase
    {
        private Dictionary<EffectKind, Effect> _effects;
        private readonly IPlayerEffectsDb _effectsDb;
        public Dictionary<EffectKind, Effect> Effects => _effects;

        public EffectContainerComponent(IPlayerEffectsDb effectsDb)
        {
            _effectsDb = effectsDb;
        }

        public override async Task Init(string owner)
        {
            _effects = await _effectsDb.GetEffects(owner);
           await base.Init(owner);
        }

        public double GetAggregatedEffect(EffectKind kind)
        {
            double boost = 1;

            if (_effects.TryGetValue(kind, out var effect))
                boost = effect.BoostPrecent;

            return boost;
        }

        public void AddEffect(Effect effect)
        {
            if (_effects.ContainsKey(effect.Kind) is not true)
            {
                _effects.Add(effect.Kind, effect);
            }
            else
            {
                _effects[effect.Kind].BoostPrecent += effect.BoostPrecent;
            }
        }

        public Task SaveChanges()
        {
            return _effectsDb.SaveEffects(_effects, Owner);
        }
    }
}
