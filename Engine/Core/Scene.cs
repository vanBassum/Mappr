namespace EngineLib.Core
{
    public class Scene
    {
        public GameObject RootObject { get; } = new GameObject();


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
    }
}

