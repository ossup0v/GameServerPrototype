using ServerPrototype.Common.Models;

namespace ServerPrototype.Core.Data
{
    public class ContentProvider
    {
        private ContentProvider() { }
        public static ContentProvider Instance = new ContentProvider();

        public Dictionary<int, FarmConstruction> GetFarmConstructions()
        {
            return new Dictionary<int, FarmConstruction>
            {
                [0] =
                    new FarmConstruction
                    {
                        Id = 0,
                        Levels = new Dictionary<int, FarmConstructionLevel>
                        {
                            [1] = new FarmConstructionLevel
                            {
                                Level = 1,
                                Effects = new List<Effect> { new Effect { Kind = EffectKind.ResourceProduction, BoostPrecent = 0.2 } },
                                ResourceRequirements = new List<ResourceRequirement> { new ResourceRequirement { ResourceId = 1, RequirementAmountOfResource = 100 } },
                                ProductionResources = new Dictionary<int, ulong>
                                {
                                    [1] = 100,
                                    [2] = 100
                                },
                                ConstructTimeSec = 3,
                            },
                            [2] = new FarmConstructionLevel
                            {
                                Level = 2,
                                Effects = new List<Effect> { new Effect { Kind = EffectKind.ResourceProduction, BoostPrecent = 0.4 } },
                                ProductionResources = new Dictionary<int, ulong>
                                {
                                    [1] = 300,
                                    [2] = 300
                                },
                                ResourceRequirements = new List<ResourceRequirement>
                                {
                                    new ResourceRequirement { ResourceId = 1, RequirementAmountOfResource = 1000},
                                    new ResourceRequirement { ResourceId = 2, RequirementAmountOfResource = 1000}
                                },
                                ConstructTimeSec = 5
                            }
                        }
                    },

                [1] =
                    new FarmConstruction
                    {
                        Id = 0,
                        Levels = new Dictionary<int, FarmConstructionLevel>
                        {
                            [1] = new FarmConstructionLevel
                            {
                                Level = 1,
                                Effects = new List<Effect> { new Effect { Kind = EffectKind.ResourceProduction, BoostPrecent = 0.2 } },
                                ResourceRequirements = new List<ResourceRequirement> { new ResourceRequirement { ResourceId = 1, RequirementAmountOfResource = 100 } },
                                ConstructTimeSec = 3,
                            },
                            [2] = new FarmConstructionLevel
                            {
                                Level = 2,
                                Effects = new List<Effect> { new Effect { Kind = EffectKind.ResourceProduction, BoostPrecent = 0.4 } },
                                ResourceRequirements = new List<ResourceRequirement>
                                {
                                    new ResourceRequirement { ResourceId = 1, RequirementAmountOfResource = 1000},
                                    new ResourceRequirement { ResourceId = 2, RequirementAmountOfResource = 1000}
                                },
                                ProductionResources = new Dictionary<int, ulong>
                                {
                                    [1] = 300,
                                    [2] = 300
                                },
                                ConstructTimeSec = 5
                            }
                        }
                    },
            };
        }
    }
}
