
namespace EngineLib.Core
{
    public class MonoBehaviour : IComponent
    {
        public GameObject GameObject { get; set; }
        public MonoBehaviour()
        {
        }

        public virtual void Start() { }
        public virtual void Update() { }

    }

}
