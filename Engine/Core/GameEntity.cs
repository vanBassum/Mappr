

using System.Diagnostics;
using System.Xml.Serialization;

namespace EngineLib.Core
{

    public class GameEntity
    {
        private List<IComponent> components = new List<IComponent>();
        private List<GameEntity> children = new List<GameEntity>();

        public Transform Transform { get; } = new Transform();
        public GameEntity? Parent { get; private set; }

        public T AddComponent<T>(T component) where T : IComponent
        {
            component.Entity = this;
            components.Add(component);
            return component;
        }

        public GameEntity AddChild(GameEntity child)
        {
            child.Parent = this;
            children.Add(child);
            return child;
        }

        public T? GetComponent<T>() where T : IComponent
        {
            return GetComponents<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetComponents<T>() where T : IComponent
        {
            return components.OfType<T>();
        }

        public IEnumerable<GameEntity> GetChildren() => children;
        public IEnumerable<IComponent> GetComponents() => components;

        public static GameEntity Empty => new GameEntity();
    }
}
