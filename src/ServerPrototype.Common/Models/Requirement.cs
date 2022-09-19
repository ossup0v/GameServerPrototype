using ServerPrototype.Shared;

namespace ServerPrototype.Common.Models
{
#warning TODO!
    public abstract class RequirementBase
    {

    }

    public class ResourceRequirement : RequirementBase
    {
        public ResourceType Resource { get; set; }
        public ulong RequirementAmountOfResource { get; set; }
    }
}
