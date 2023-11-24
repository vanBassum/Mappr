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

            var cam = new Camera(pictureBox1);
            scene.RootObject.AddChild(cam);
            cam.AddComponent<Bobbing2>();




            return scene;
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
                
                if (GameObject.Transform.Position.X < 20)
                    velocity = Vector2.UnitX * speed;
            }
        }

        public class Bobbing2 : MonoBehaviour
        {
            float speed = 10f;      //px per second
            Vector2 velocity = Vector2.Zero;

            public override void Update()
            {
                GameObject.Transform.Position += velocity * Time.DeltaTime;

                if (GameObject.Transform.Position.Y > 100)
                    velocity = Vector2.UnitY * -speed;

                if (GameObject.Transform.Position.Y < 20)
                    velocity = Vector2.UnitY * speed;
            }
        }


        public class Coin : GameObject
        {
            LinesRenderer? renderer;
            Bobbing? bobbing;

            public Coin()
            {
                renderer = AddComponent<LinesRenderer>();
                renderer.Points = CreateCircleMesh(20).ToArray();

                bobbing = AddComponent<Bobbing>();
                Transform.Position = new Vector2(0, 100);
            }

            List<Vector2> CreateCircleMesh(float radius)
            {
                // Number of points to create the circle
                int numPoints = 6;
                List<Vector2> pointsList = new List<Vector2>();

                for (int i = 0; i < numPoints + 1; i++)
                {
                    float angle = (float)i / numPoints * 2 * MathF.PI;
                    float x = radius * MathF.Cos(angle);
                    float y = radius * MathF.Sin(angle);
                    pointsList.Add(new Vector2 { X = x, Y = y });
                }
                return pointsList;
            }
        }


    }
}
