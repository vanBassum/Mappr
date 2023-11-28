
namespace EngineLib.Core
{
    public interface IComponent
    {
        public GameEntity Entity { get; set; }
        public void Awake() { }
        public void Start() { }
        public void Update() { }
    }
}
