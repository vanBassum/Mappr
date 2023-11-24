using EngineLib.Core;
using Mappr.Meuk;
using System.Numerics;
using System.Runtime.Intrinsics;

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


            for (int i = 0; i < 10; i++)
            {
                var coin = new Coin();
                scene.RootObject.AddChild(coin);

                MoveToRandom moveToRandom = coin.AddComponent<MoveToRandom>();
                moveToRandom.Max = new Vector2(pictureBox1.Width, pictureBox1.Height);

                //coin.bobbing.EndPos = new Vector2(Random.Next(pictureBox1.Width), Random.Next(pictureBox1.Height));
                coin.rotator.speed = (float)(RANDOM.Random.NextDouble() * Math.PI);
                coin.Transform.Position = new Vector2(RANDOM.Random.Next(pictureBox1.Width), RANDOM.Random.Next(pictureBox1.Height));

            }


            //scene.RootObject.AddChild(new Coin());
            var cam = new Camera(pictureBox1);
            cam.Transform.Scale = Vector2.One;
            scene.RootObject.AddChild(cam);
            //cam.AddComponent<Bobbing2>();

            return scene;
        }




    }


}
