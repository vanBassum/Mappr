namespace EngineLib.Core
{
    public class GameObject
    {
        private readonly List<IComponent> Components = new List<IComponent>();

        public void AddComponent(IComponent component)
        {
            Components.Add(component);

        }

        public void Update()
        {
            OnUpdate();
            foreach (var component in Components)
            {
                component.Update();
            }
        }

        protected virtual void OnUpdate()
        {
            
        }

        public T? GetComponent<T>() where T : class, IComponent
        {
            return Components.Find(c => c is T) as T;
        }

        public IEnumerable<T> GetComponents<T>() where T : class, IComponent
        {
            return Components.FindAll(c => c is T).Cast<T>();
        }
    }
}
