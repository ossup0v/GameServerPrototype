using ServerPrototype.Shared;

namespace ServerPrototype.Common.Models
{
    //TODO write effect resource production, effect build speed and ect!
    public class Effect
    {
        public Effect() { }

        public Effect(EffectType kind, double boostPrecent)
        {
            Kind = kind;
            BoostPrecent = boostPrecent;
        }
        public EffectType Kind { get; set; }
        public double BoostPrecent { get; set; }
    }

    public class ResourceProductionEffect : Effect
    {
        public ResourceProductionEffect() { }
        public ResourceProductionEffect(ResourceType resource, double boostPrecent) : base(EffectType.ResourceProduction, boostPrecent)
        {
            Resource = resource;
        }

        public ResourceType Resource { get; set; }
    }

    public enum EffectType
    {
        ResourceProduction
    }
}
