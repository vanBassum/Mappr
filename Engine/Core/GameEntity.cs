

using System.Diagnostics;
using System.Xml.Serialization;

namespace EngineLib.Core
{

    public class GameEntity : IStartable, IUpdateable, IAwakable
    {
        private List<IComponent> components = new List<IComponent>();
        private List<GameEntity> children = new List<GameEntity>();

        public Transform Transform { get; private set; }
        public GameEntity? Parent { get; private set; }
        public Scene? Scene { get; private set; }
        public IEnumerable<GameEntity> Children => children;


        public GameEntity()
        {
            Transform = new Transform();
        }

        public GameEntity(Scene scene)
        {
            Transform = new Transform();
            Scene = scene;
        }

        public T AddChild<T>() where T : GameEntity, new()
        {
            return AddChild(new T());
        }

        public T AddChild<T>(T child) where T : GameEntity
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
            child.Scene = Scene;
            children.Add(child);
            child.Awake();
            PassNew(child);
            return child;
        }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void Awake() { }

        public T AddComponent<T>() where T : IComponent, new()
        {
            return AddComponent(new T());   
        }

        public T AddComponent<T>(T component) where T : IComponent
        {
            if (component is GameScript mono)
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

        public static GameEntity? RootEntity { private get; set; }


        public T? GetComponent<T>() where T : IComponent
        {
            return GetComponents<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetComponents<T>() where T : IComponent
        {
            foreach (var component in components.OfType<T>())
            {
                yield return component;
            }
        }

        public static IEnumerable<GameEntity> FindObjectsThatHaveComponentOfType<T>() where T : IComponent
        {
            if (RootEntity == null)
            {
                yield break;
            }

            Queue<GameEntity> queue = new Queue<GameEntity>();
            queue.Enqueue(RootEntity);

            while (queue.Count > 0)
            {
                GameEntity obj = queue.Dequeue();

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
