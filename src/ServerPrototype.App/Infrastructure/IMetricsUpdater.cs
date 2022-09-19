using App.Metrics;

namespace ServerPrototype.App.Infrastructure
{
    public interface IMetricsUpdater
    {
        void Increment(string name, Unit units);
        void Decrement(string name, Unit units);

        IDisposable Timing(string name);
    }
}
