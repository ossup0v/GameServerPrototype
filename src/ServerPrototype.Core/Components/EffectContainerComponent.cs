using ServerPrototype.Common.Models;
using ServerPrototype.DAL.Api;
using ServerPrototype.Shared;

namespace ServerPrototype.Core.Components
{
    public class EffectContainerComponent : ComponentBase
    {
        private Dictionary<EffectType, Effect> _effects;
        private readonly IPlayerEffectsDb _effectsDb;
        public Dictionary<EffectType, Effect> Effects => _effects;

        public EffectContainerComponent(IPlayerEffectsDb effectsDb)
        {
            _effectsDb = effectsDb;
        }

        public override async Task Init(string owner)
        {
            _effects = await _effectsDb.GetEffects(owner);
            await base.Init(owner);
        }

        public double GetAggregatedEffect(EffectType kind)
        {
            double boost = 1;

            if (_effects.TryGetValue(kind, out var effect))
                boost = effect.BoostPrecent;

            return boost;
        }

        public Dictionary<ResourceType, double> GetAggregatedResourceProductionEffects()
        {
            var result = new Dictionary<ResourceType, double>();

            foreach (var effect in _effects)
            {
                if (effect.Value is ResourceProductionEffect resourceProductionEffect)
                {
                    if (!result.ContainsKey(resourceProductionEffect.Resource))
                    {
                        result.Add(resourceProductionEffect.Resource, 0D);
                    }
                    result[resourceProductionEffect.Resource] += resourceProductionEffect.BoostPrecent;
                }
            }

            return result;
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
