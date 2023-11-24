using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace EngineLib.Core
{
    public class Scene
    {
        public Queue<IStartable> Startables { get; } = new Queue<IStartable> { };
        public GameObject RootObject { get; }

        public Scene() { 
            RootObject = new GameObject(this);
        }  

        public IEnumerable<GameObject> GetGameObjects()
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            queue.Enqueue(RootObject);

            while (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();
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

