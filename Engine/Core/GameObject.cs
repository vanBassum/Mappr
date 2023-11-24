namespace EngineLib.Core
{

    public class GameObject
    {
        private List<IComponent> components = new List<IComponent>();
        private List<GameObject> children = new List<GameObject>();

        public Transform Transform { get; private set; }
        public GameObject? Parent { get; private set; }

        public IEnumerable<GameObject> Children => children;


        public GameObject()
        {
            Transform = new Transform();
        }

        public void AddChild(GameObject child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (child.Parent != null)
            {
                throw new InvalidOperationException("GameObject already has a parent.");
            }

            child.Parent = this;
            children.Add(child);
            child.Start();
        }

        protected T AddComponent<T>() where T : IComponent, new()
        {
            T instance = new T();
            components.Add(instance);
            return instance;
        }

        public virtual void Start() { }
        public virtual void Update() { }


        //public T? GetComponent<T>() where T : class, IComponent
        //{
        //    return Components.Find(c => c is T) as T;
        //}
        //
        //public IEnumerable<T> GetComponents<T>() where T : class, IComponent
        //{
        //    return Components.FindAll(c => c is T).Cast<T>();
        //}
    }

}
