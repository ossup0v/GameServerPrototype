using ServerPrototype.Shared;

namespace ServerPrototype.Common.Models
{
    public class FarmConstruction
    {
        public int Id { get; set; }
        public Dictionary<int, FarmConstructionLevel> Levels { get; set; }
        public FarmConstructionLevel FirstLevel => Levels[Levels.Keys.Min()];
    }

    public class FarmConstructionLevel
    {
        public int Level { get; set; }
        public List<Effect> Effects { get; set; }
        public List<RequirementBase> Requirements { get; set; }
        public Dictionary<ResourceType, ulong> ResourceToSpend { get; init; }
        public Dictionary<ResourceType, ulong> ProductionResources { get; set; }
        public int ConstructTimeSec { get; set; }
        public string Asset { get; set; }
        public Point Point { get; set; }

        public FarmConstructionLevel(int level, List<Effect> effects, List<RequirementBase> resourceRequirements, Dictionary<ResourceType, ulong> productionResources, int constructTimeSec, string asset)
        {
            Level = level;
            Effects = effects;
            Requirements = resourceRequirements;
            ProductionResources = productionResources;
            ConstructTimeSec = constructTimeSec;
            Asset = asset;

            ResourceToSpend = Requirements
                .Where(req => req is ResourceRequirement)
                .Select(x => (ResourceRequirement)x)
                .ToDictionary(x => x.Resource, x => x.RequirementAmountOfResource);
        }
    }
}
