using EngineLib.Core;
using EngineLib.Rendering;
using EngineLib.Statics;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks.Dataflow;
using static Mappr.Form1;

namespace Mappr
{
    public partial class Form1 : Form
    {
        Engine engine;
        public static Random Random = new Random();
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


            for (int i = 0; i < 100; i++)
            {
                var coin = new Coin();
                scene.RootObject.AddChild(coin);

                MoveToRandom moveToRandom = coin.AddComponent<MoveToRandom>();
                moveToRandom.Max = new Vector2(pictureBox1.Width, pictureBox1.Height);

                //coin.bobbing.EndPos = new Vector2(Random.Next(pictureBox1.Width), Random.Next(pictureBox1.Height));
                coin.rotator.speed = (float)(Random.NextDouble() * Math.PI);
                coin.Transform.Position = new Vector2(Random.Next(pictureBox1.Width), Random.Next(pictureBox1.Height));

            }


            //scene.RootObject.AddChild(new Coin());
            var cam = new Camera(pictureBox1);
            scene.RootObject.AddChild(cam);
            //cam.AddComponent<Bobbing2>();

            return scene;
        }

        public class MoveToRandom : MonoBehaviour
        {
            public float Speed { get; set; } = 25f;      //px per second
            public Vector2 Max { get; set; }

            Vector2 moveTo;

            StaticLinesRenderer LinesRenderer { get; set; }

            public override void Start()
            {
                LinesRenderer = GameObject.AddComponent<StaticLinesRenderer>();
                LinesRenderer.Pen = Pens.Red;   
                NewDestination();
            }

            public override void Update()
            {
                var distance = Speed * Time.DeltaTime;
                var step = moveTo - GameObject.Transform.Position;

                if (step.Length() < distance)
                {
                    NewDestination();
                }
                else
                {
                    step = Vector2.Normalize(step) * distance;
                }

                GameObject.Transform.Position += step;

                LinesRenderer.Points = new Vector2[] { GameObject.Transform.Position, moveTo };
            }

            void NewDestination()
            {
                moveTo = new Vector2(Random.Next((int)Max.X), Random.Next((int)Max.Y));
            }

        }

        public class Bobbing : MonoBehaviour
        {
            float speed = 25f;      //px per second
            Vector2 startPos;
            public Vector2 EndPos { get; set; }

            Vector2 moveTo;

            public override void Start()
            {
                startPos = GameObject.Transform.Position;
                moveTo = EndPos;
            }

            public override void Update()
            {
                var distance = speed * Time.DeltaTime;
                var step = moveTo - GameObject.Transform.Position;

                if (step.Length() < distance)
                {
                    if (moveTo == EndPos)
                        moveTo = startPos;
                    else
                        moveTo = EndPos;
                }
                else
                {
                    step = Vector2.Normalize(step) * distance;
                }

                GameObject.Transform.Position += step;
            }
        }

        public class Rotator : MonoBehaviour
        {
            public float speed = 1;
            public override void Update()
            {
                GameObject.Transform.Rotation += speed * Time.DeltaTime;
            }
        }

        public class Coin : GameObject
        {
            LinesRenderer? renderer;
            public Rotator? rotator;

            public override void Awake()
            {
                renderer = AddComponent<LinesRenderer>();
                renderer.Points = CreateCircleMesh(10).ToArray();
                rotator = AddComponent<Rotator>();
            }

            public override void Start()
            {

            }


            List<Vector2> CreateCircleMesh(float radius)
            {
                // Number of points to create the circle
                int numPoints = 6;
                List<Vector2> pointsList = new List<Vector2>();

                for (int i = 0; i < numPoints; i++)
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
    public static class Ext
    {
        public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
        {
            if (obj.InvokeRequired)
            {
                var args = new object[0];
                obj.Invoke(action, args);
            }
            else
            {
                action();
            }
        }
    }

    public class FloatAverageCalculator
    {
        private float[] values;
        private int index;
        private float sum;

        public FloatAverageCalculator(int capacity)
        {
            values = new float[capacity];
            index = 0;
            sum = 0;
        }

        public float Add(float value)
        {
            sum -= values[index];
            values[index] = value;
            sum += value;
            index++;
            if (index >= values.Length)
                index = 0;
            return sum / (float)values.Length;

        }
    }


}
