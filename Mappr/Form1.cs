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

            engine = new Engine();
            engine.Load(CreateScene());
            engine.onFrame += Engine_onFrame;
        }

        private void Engine_onFrame(object? sender, TimeSpan e)
        {
            this.InvokeIfRequired(() =>
            {

                this.Text = avg.Add((float)e.TotalMilliseconds).ToString();
            });
        }

        Scene CreateScene()
        {
            Scene scene = new Scene();

            //rect1.Transform.Rotation = 10 * MathF.PI / 180;

            var man = scene.RootEntity.AddChild<Manager>();

            var cam1 = new Camera(pictureBox1);
            cam1.Transform.Scale = Vector2.One * 1f;
            scene.RootEntity.AddChild(cam1);

            var cam2 = new Camera(pictureBox2);
            //cam2.Transform.Position = new Vector2(-100, -100);
            //cam2.Transform.Rotation = MathF.PI * 45 / 180;
            cam2.Transform.Scale = Vector2.One * 10f;
            scene.RootEntity.AddChild(cam2);
            return scene;
        }

    }


    public class Manager : GameEntity
    {
        Dot dot;
        public override void Start()
        {
            AddPlayer(this, new Vector2(100, 100));
            dot = AddChild<Dot>();
        }

        public override void Update()
        {
            var entities = Scene?.GetGameEntities().Where(a => a.GetComponent<ICollider>() != null);
            if (entities != null && Input.Mouse.IsValid)
            {
                bool found = false;
                foreach (var entity in entities)
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

            if(Input.Mouse.IsValid)
            {
                dot.Transform.Position = Input.Mouse.Position;
            }
        }


        void AddPlayer(GameEntity entity, Vector2 pos)
        {
            var rect1 = entity.AddChild<MyPlayer>();

            rect1.Transform.Position = pos;
            rect1.AddComponent<Bobbing>();
            rect1.AddComponent<Rotator>();
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


    public class MyPlayer : GameEntity
    {
        MeshCollider collider;
        MeshRenderer renderer;
        public MyPlayer()
        {
            Mesh mesh1 = new Mesh();
            mesh1.AddRectangle(new Vector2(-25, -25), new Vector2(50, 50));  //Head

            Mesh mesh2 = new Mesh();
            mesh2.AddLine(new Vector2(0, 25), new Vector2(0, 100));


            collider = new MeshCollider();
            renderer = new MeshRenderer();


            collider.Meshes.AddRange(new Mesh[] { mesh1, mesh2 });
            renderer.Meshes.AddRange(new Mesh[] { mesh1, mesh2 });

        }


        public override void Awake()
        {
            AddComponent(collider);
            AddComponent(renderer);
        }


        public override void Update()
        {

            //bool collide = collider.Collides(Input.Mouse.WorldPosition, Transform);

            //renderer.Pen = collide? Pens.Red : Pens.Black;

        }
    }
}
