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


            var rect1 = scene.RootObject.AddChild<MyPlayer>();
            rect1.Transform.Position = new Vector2(100, 100);
            rect1.Transform.Rotation = 10 * MathF.PI / 180;



            var cam1 = new Camera(pictureBox1);
            cam1.Transform.Scale = Vector2.One * 1f;
            scene.RootObject.AddChild(cam1);

            var cam2 = new Camera(pictureBox2);
            cam2.Transform.Scale = Vector2.One * 2f;
            scene.RootObject.AddChild(cam2);
            return scene;
        }
    }

    public class MyPlayer : GameObject
    {
        MeshCollider collider;
        MeshRenderer renderer;
        public MyPlayer()
        {
            Mesh mesh = new Mesh();
            mesh.AddRectangle(new Vector2(-50, -50), new Vector2(50, 0));  //Head

            collider = new MeshCollider();
            renderer = new MeshRenderer();


            collider.Meshes.Add(mesh);
            renderer.Meshes.Add(mesh);

            //     shapes.Add(new ShapeRectangle { PointA = new Vector2(-50, -50), PointB = new Vector2(50, 0) })
            //
            // shapes.Add(new ShapeLine { PointA = new Vector2(0, 0), PointB = new Vector2(0, -50) })          //Body
            //
            //
            //
            // collider = new ShapeCollider();
            //     collider.Shapes = shapes;
            //     renderer = new ShapeRenderer();
            //     renderer.Shapes = shapes;
        }


        public override void Awake()
        {
            AddComponent(collider);
            AddComponent(renderer);



        }


    }



}
