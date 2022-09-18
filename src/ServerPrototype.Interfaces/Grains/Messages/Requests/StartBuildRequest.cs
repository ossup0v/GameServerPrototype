using Orleans.Concurrency;
using ServerPrototype.Shared;

namespace ServerPrototype.Interfaces.Grains
{
    [Immutable]
    public record StartBuildRequest(Point Point, int ConstructionId);
}
