using ServerPrototype.Shared;
using System.Collections.Generic;

namespace ServerPrototype.Common.Models
{
#warning TODO!
    public abstract class RequirementBase
    {
        public abstract bool Validate(IRequirementContext resolver);
    }

    public class ResourceRequirement : RequirementBase
    {
        public ResourceType Resource { get; set; }
        public ulong RequirementAmountOfResource { get; set; }

        public override bool Validate(IRequirementContext context)
        {
            return context.Resources.ContainsKey(Resource) && context.Resources[Resource] >= RequirementAmountOfResource;
        }
    }

    public interface IRequirementContext
    {
        IReadOnlyDictionary<ResourceType, ulong> Resources { get; }
    }
}
