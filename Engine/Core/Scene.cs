using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace EngineLib.Core
{
    public class Scene
    {
        public Queue<IStartable> Startables { get; } = new Queue<IStartable> { };
        public GameEntity RootEntity { get; }

        public Scene() { 
            RootEntity = new GameEntity(this);
        }  

        public IEnumerable<GameEntity> GetGameEntities()
        {
            Queue<GameEntity> queue = new Queue<GameEntity>();
            queue.Enqueue(RootEntity);

            while (queue.Count > 0)
            {
                GameEntity obj = queue.Dequeue();
                yield return obj;

                foreach (var child in obj.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public void PassNew(IStartable startable)
        {
            Startables.Enqueue(startable);
        }
    }
}

