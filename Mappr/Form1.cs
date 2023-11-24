using EngineLib.Core;
using EngineLib.Rendering;
using EngineLib.Statics;
using System.Numerics;
using System.Threading.Tasks.Dataflow;

namespace Mappr
{
    public partial class Form1 : Form
    {
        Engine engine;

        public Form1()
        {
            InitializeComponent();

            engine = new Engine();
            engine.Load(CreateScene());
        }

        Scene CreateScene()
        {
            Scene scene = new Scene();

            scene.RootObject.AddChild(new Coin());

            return scene;
        }


        public class MonoBehaviour : IComponent
        {
            public GameObject GameObject { get; set; }
            public virtual void Start() { }
            public virtual void Update() { }
        }




        public class Bobbing : MonoBehaviour
        {
            float speed = 10f;      //px per second
            Vector2 velocity = Vector2.Zero;

            public override void Update()
            {
                GameObject.Transform.Position += velocity * Time.DeltaTime;

                if (GameObject.Transform.Position.X > 100)
                    velocity = Vector2.UnitX * -speed;

                if (GameObject.Transform.Position.X < 10)
                    velocity = Vector2.UnitX * speed;
            }
        }



        public class Coin : GameObject
        {
            MeshRenderer? renderer;
            Bobbing? bobbing;

            public override void Start()
            {
                renderer = AddComponent<MeshRenderer>();
                renderer.Mesh = CreateCircleMesh(5);

                bobbing = AddComponent<Bobbing>();  
            }

            Mesh CreateCircleMesh(float radius)
            {
                // Assuming some circle mesh creation logic here
                Mesh mesh = new Mesh();
                mesh.Points = new Vector2[] { new Vector2 { X = 0, Y = 0 }, new Vector2 { X = radius, Y = 0 } };
                return mesh;
            }


        }
    }
}
