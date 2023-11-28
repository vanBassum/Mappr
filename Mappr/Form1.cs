using EngineLib.Core;
using EngineLib.Rendering;
using EngineLib.Statics;
using Mappr.Meuk;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Design.Behavior;

namespace Mappr
{
    public partial class Form1 : Form
    {
        Engine engine;

        FloatAverageCalculator avg = new FloatAverageCalculator(15);
        public Form1()
        {
            InitializeComponent();

            GameEntity root = CreateScene();
            engine = new Engine(root);
            engine.onFrame += Engine_onFrame;
        }

        private void Engine_onFrame(object? sender, TimeSpan e)
        {
            this.InvokeIfRequired(() =>
            {
                this.Text = avg.Add((float)e.TotalMilliseconds).ToString();
            });
        }

        GameEntity CreateScene()
        {
            GameEntity root = new GameEntity();

            var square1 = root.AddChild(new GameEntity());
            square1.AddComponent(new MeshRenderer());
            square1.AddComponent(Mesh.Rectangle(1, 1));
            square1.Transform.Scale = Vector2.One * 20f;

            var camObject1 = root.AddChild(new GameEntity());
            camObject1.AddComponent(new Camera(pictureBox1));

           // var camObject2 = root.AddChild(new GameEntity());
           // camObject2.AddComponent(new Camera(pictureBox2));

            return root;
        }
    }






    /*
    public class Manager : GameEntity
    {
        Dot dot;
        public override void Start()
        {
            dot = AddChild<Dot>();
        }

        public override void Update()
        {
            var entities = Scene?.GetGameEntities().Where(a => a.GetComponent<ICollider>() != null);
            if (entities != null && Input.Mouse.IsValid)
            {
                bool found = false;
                foreach (var entity in entities.Reverse())
                {
                    var collider = entity.GetComponent<ICollider>();
                    var renderer = entity.GetComponent<MeshRenderer>();
                    bool collide = collider?.Collides(Input.Mouse.Position, entity.Transform) ?? false;

                    bool active = !found && collide;
                    found |= collide;

                    if (renderer != null)
                        renderer.Pen = active ? Pens.Red : Pens.Black;
                }
            }

            if (Input.Mouse.IsValid && Input.Mouse.Camera != null)
            {
                if (Input.Mouse.WheelDelta < 0)
                    Input.Mouse.Camera.Transform.Scale *= 2;
                else if (Input.Mouse.WheelDelta > 0)
                    Input.Mouse.Camera.Transform.Scale /= 2;
            }

            if (Input.Mouse.IsValid)
            {
                dot.Transform.Position = Input.Mouse.Position;
            }
        }
    }





    public class Component : GameEntity
    {
        //private readonly List<Port> ports = new List<Port>();
        MeshRenderer renderer;

        public override void Awake()
        {
            Mesh mesh = new Mesh();
            mesh.AddRectangle(new Vector2(-50, -50), new Vector2(100, 100));
            renderer = AddComponent<MeshRenderer>();
            renderer.Meshes.Add(mesh);

            AddChild<Port>().Transform.Position = new Vector2(45, 15);
            AddChild<Port>().Transform.Position = new Vector2(45, 0);
            AddChild<Port>().Transform.Position = new Vector2(45, -15);
        }


    }

    public class Port : GameEntity 
    {
        MeshRenderer renderer;
        public override void Awake()
        {
            Mesh mesh = new Mesh();
            mesh.AddRectangle(new Vector2(-5, -5), new Vector2(10, 10));
            renderer = AddComponent<MeshRenderer>();
            renderer.Meshes.Add(mesh);
        }
    }



















    public class Dot : GameEntity
    {
        MeshRenderer renderer;
        public Dot()
        {
            Mesh mesh1 = new Mesh();
            mesh1.AddRectangle(new Vector2(-5, -5), new Vector2(10, 10));

            renderer = new MeshRenderer();
            renderer.Meshes.AddRange(new Mesh[] { mesh1 });

        }

        public override void Awake()
        {
            AddComponent(renderer);
        }
    }
    */
}
