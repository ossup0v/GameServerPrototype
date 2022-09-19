using Orleans.Concurrency;
using ServerPrototype.Shared;

namespace ServerPrototype.Actors.Grains.Messages.Requests
{
    [Immutable]
    public record StartBuildRequest(Point Point, int ConstructionId, int Level);
}
