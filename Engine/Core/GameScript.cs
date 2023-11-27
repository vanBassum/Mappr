namespace EngineLib.Core
{



    public class GameScript : IComponent, IStartable, IUpdateable, IAwakable
    {
        public GameEntity GameObject { get; set; }
        public GameScript()
        {
        }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void Awake() { }
    }

}
