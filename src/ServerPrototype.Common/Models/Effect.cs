namespace ServerPrototype.Common.Models
{
    public class Effect
    {
        public EffectKind Kind { get; set; }
        public double BoostPrecent { get; set; }
    }

    public enum EffectKind
    {
        ResourceProduction
    }
}
