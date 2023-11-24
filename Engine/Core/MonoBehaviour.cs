
namespace EngineLib.Core
{



    public class MonoBehaviour : IComponent, IStartable, IUpdateable, IAwakable
    {
        public GameObject GameObject { get; set; }
        public MonoBehaviour()
        {
        }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void Awake() { }
    }

}
