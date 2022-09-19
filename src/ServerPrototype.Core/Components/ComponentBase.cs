namespace ServerPrototype.Core.Components
{
    public abstract class ComponentBase
    {
        public string Owner { get; private set; }
        public virtual Task Init(string owner)
        {
            Owner = owner;
            return Task.CompletedTask;
        }
    }
}
