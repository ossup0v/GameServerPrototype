using App.Metrics.Counter;
using App.Metrics.Timer;
using App.Metrics;
using Microsoft.Extensions.ObjectPool;

namespace ServerPrototype.App.Infrastructure
{
    public class AppMetricsProxy : IMetricsUpdater
    {
        private readonly IMetrics _metrics;
        private readonly ObjectPool<CounterOptions> _counterOptionsPool;
        private readonly ObjectPool<TimerOptions> _timerPool;

        public AppMetricsProxy(IMetrics metrics, ObjectPool<CounterOptions> counterOptionsPool, ObjectPool<TimerOptions> timerPool)
        {
            _metrics = metrics;
            _counterOptionsPool = counterOptionsPool;
            _timerPool = timerPool;
        }

        public IDisposable Timing(string name)
        {
            var opts = _timerPool.Get();
            opts.Name = name;

            var timer = _metrics.Measure.Timer.Time(opts);
            _timerPool.Return(opts);

            return timer;
        }

        public void Increment(string name, Unit units)
        {
            var opts = _counterOptionsPool.Get();
            opts.Name = name;
            opts.MeasurementUnit = Unit.Requests;

            _metrics.Measure.Counter.Increment(opts);

            _counterOptionsPool.Return(opts);
        }

        public void Decrement(string name, Unit units)
        {
            var opts = _counterOptionsPool.Get();
            opts.Name = name;
            opts.MeasurementUnit = Unit.Requests;

            _metrics.Measure.Counter.Decrement(opts);

            _counterOptionsPool.Return(opts);
        }
    }
}
