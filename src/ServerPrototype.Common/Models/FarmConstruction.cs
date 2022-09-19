using ServerPrototype.Shared;

namespace ServerPrototype.Common.Models
{
    public class FarmConstruction
    {
        public int Id { get; set; }
        public Point Point { get; set; }
        public Dictionary<int, FarmConstructionLevel> Levels { get; set; }
    }

    public class FarmConstructionLevel
    {
        public int Level { get; set; }
        public List<Effect> Effects { get; set; }
#warning TODO rewrite it to RequirementBase
        public List<ResourceRequirement> ResourceRequirements { get; set; }
        public Dictionary<int, ulong> ProductionResources { get; set; }
        public int ConstructTimeSec { get; set; }
        public string Asset { get; set; }
    }
}
