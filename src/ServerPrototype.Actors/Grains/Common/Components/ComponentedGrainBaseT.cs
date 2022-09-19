using Orleans;
using ServerPrototype.Core.Components;

namespace ServerPrototype.Actors.Grains.Common.Components
{
    public abstract class ComponentedGrainBase<TState> : Grain<TState>
    {
        private Dictionary<Type, ComponentBase> _components = new Dictionary<Type, ComponentBase>();

        public override Task OnActivateAsync()
        {
            return Init();
        }

        protected virtual Task Init()
        {
            return Task.CompletedTask;
        }

        public T GetComponent<T>() where T : ComponentBase
        {
            _components.TryGetValue(typeof(T), out var component);
            return (T)component;
        }

        protected Task AddComponent<T>(T component, string owner) where T : ComponentBase
        {
            _components.TryAdd(typeof(T), component);
            return component.Init(owner);
        }
    }
}
