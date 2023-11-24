

using System.Diagnostics;
using System.Xml.Serialization;

namespace EngineLib.Core
{

    public class GameObject : IStartable, IUpdateable, IAwakable
    {
        private List<IComponent> components = new List<IComponent>();
        private List<GameObject> children = new List<GameObject>();

        public Transform Transform { get; private set; }
        public GameObject? Parent { get; private set; }
        public Scene? Scene { get; private set; }
        public IEnumerable<GameObject> Children => children;


        public GameObject()
        {
            Transform = new Transform();
        }

        public GameObject(Scene scene)
        {
            Transform = new Transform();
            Scene = scene;
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
            child.Awake();
            PassNew(child);
        }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void Awake() { }

        public T AddComponent<T>() where T : IComponent, new()
        {
            T component = new T();

            if (component is MonoBehaviour mono)
            {
                mono.GameObject = this;
                mono.Awake();
                PassNew(mono);
            }
            components.Add(component);
            return component;
        }

        public void PassNew(IStartable startable)
        {
            Parent?.PassNew(startable);
            Scene?.PassNew(startable);
        }

        public static GameObject? RootObject { private get; set; }


        public IEnumerable<T> GetComponent<T>() where T : IComponent
        {
            foreach (var component in components.OfType<T>())
            {
                yield return component;
            }
        }

        public static IEnumerable<GameObject> FindObjectsThatHaveComponentOfType<T>() where T : IComponent
        {
            if (RootObject == null)
            {
                yield break;
            }

            Queue<GameObject> queue = new Queue<GameObject>();
            queue.Enqueue(RootObject);

            while (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();

                // Check if the object has the desired component type
                if (obj.components.Any(c => c is T))
                {
                    yield return obj;
                }

                foreach (var child in obj.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
    }
}
