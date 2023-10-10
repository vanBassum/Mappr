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

            engine = new Engine(pictureBox1);
            engine.Register(new MyBall());
        }


        public class MyBall : GameObject
        {
            float speed = 10f;
            Vector2 velocity = Vector2.Zero;
            Vector2 position = new Vector2(0, 20);



            public MyBall()
            {
                BasicRenderer renderer = new BasicRenderer(Render);
                this.AddComponent(renderer);
            }


            protected override void OnUpdate()
            {
                position += velocity * Time.DeltaTime;

                if (position.X > 100)
                    velocity = Vector2.UnitX * -speed;

                if (position.X < 10)
                    velocity = Vector2.UnitX * speed;
            }

            void Render(V2Graphics g)
            {
                g.DrawCircle(position, 5);
            }
        }

    }





}
