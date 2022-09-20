using ServerPrototype.Common.Models;
using ServerPrototype.Shared;
using System.Diagnostics.CodeAnalysis;

namespace ServerPrototype.Core.Data
{
    public class ContentProvider
    {
        private ContentProvider() { }
        public static ContentProvider Instance = new ContentProvider();
        private Dictionary<int, FarmConstruction> _constructions = new Dictionary<int, FarmConstruction>
        {
            [0] =
                    new FarmConstruction
                    {
                        Id = 0,
                        Levels = new Dictionary<int, FarmConstructionLevel>
                        {
                            [1] = new FarmConstructionLevel(1,
                            new List<Effect>
                                {
                                    new ResourceProductionEffect { Resource = ResourceType.Wood, BoostPrecent = 0.2 }
                                },
                            new List<RequirementBase>
                                {
                                    new ResourceRequirement { Resource = ResourceType.Metal, RequirementAmountOfResource = 100 }
                                },
                            new Dictionary<ResourceType, ulong>
                            {
                                [ResourceType.Wood] = 100,
                                [ResourceType.Stone] = 100
                            },
                            3,
                            string.Empty),
                            [2] = new FarmConstructionLevel(2,
                            new List<Effect>
                                {
                                    new ResourceProductionEffect { Resource = ResourceType.Wood, BoostPrecent = 0.2 },
                                    new ResourceProductionEffect { Resource = ResourceType.Metal, BoostPrecent = 0.2 },
                                },
                            new List<RequirementBase>
                                {
                                    new ResourceRequirement { Resource = ResourceType.Food, RequirementAmountOfResource = 100 },
                                    new ResourceRequirement { Resource = ResourceType.Metal, RequirementAmountOfResource = 100 },
                                },
                            new Dictionary<ResourceType, ulong>
                            {
                                [ResourceType.Wood] = 200,
                                [ResourceType.Stone] = 200
                            },
                            5,
                            string.Empty)
                        }
                    },

            [1] =
                    new FarmConstruction
                    {
                        Id = 1,
                        Levels = new Dictionary<int, FarmConstructionLevel>
                        {
                            [1] = new FarmConstructionLevel(1,
                            new List<Effect>
                                {
                                    new ResourceProductionEffect { Resource = ResourceType.Wood, BoostPrecent = 0.2 }
                                },
                            new List<RequirementBase>
                                {
                                    new ResourceRequirement { Resource = ResourceType.Metal, RequirementAmountOfResource = 100 }
                                },
                            new Dictionary<ResourceType, ulong>
                            {
                                [ResourceType.Wood] = 100,
                                [ResourceType.Stone] = 100
                            },
                            3,
                            string.Empty),
                            [2] = new FarmConstructionLevel(2,
                            new List<Effect>
                                {
                                    new ResourceProductionEffect { Resource = ResourceType.Wood, BoostPrecent = 0.2 },
                                    new ResourceProductionEffect { Resource = ResourceType.Metal, BoostPrecent = 0.2 },
                                },
                            new List<RequirementBase>
                                {
                                    new ResourceRequirement { Resource = ResourceType.Food, RequirementAmountOfResource = 100 },
                                    new ResourceRequirement { Resource = ResourceType.Metal, RequirementAmountOfResource = 100 },
                                },
                            new Dictionary<ResourceType, ulong>
                            {
                                [ResourceType.Wood] = 200,
                                [ResourceType.Stone] = 200
                            },
                            5,
                            string.Empty)
                        }
                    },
        };

        public Dictionary<int, FarmConstruction> GetFarmConstructions()
        {
            return _constructions;
        }

        [return: MaybeNull]
        public FarmConstruction? TryGetFarmConstruction(int constructionId)
        {
            return _constructions.TryGetValue(constructionId);
        }

        public Task<Dictionary<ResourceType, ulong>> GetFirstLoginResources()
        {
            return Task.FromResult(new Dictionary<ResourceType, ulong>
            {
                [ResourceType.Metal] = 5000,
                [ResourceType.Wood] = 5000,
                [ResourceType.Food] = 5000,
                [ResourceType.Stone] = 5000,
                [ResourceType.Silver] = 5000
            });
        }
    }
}
