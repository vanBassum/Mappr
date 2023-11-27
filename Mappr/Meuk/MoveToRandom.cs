using EngineLib.Core;
using EngineLib.Rendering;
using EngineLib.Statics;
using System.Numerics;

namespace Mappr.Meuk
{
    public class MoveToRandom : GameScript
    {
        public float Speed { get; set; } = 200f;      //px per second
        public Vector2 Max { get; set; }

        Vector2 moveTo;

        //StaticLinesRenderer LinesRenderer { get; set; }

        public override void Start()
        {
            //LinesRenderer = GameObject.AddComponent<StaticLinesRenderer>();
            //LinesRenderer.Pen = Pens.Red;
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

            //LinesRenderer.Points = new Vector2[] { GameObject.Transform.Position, moveTo };
        }

        void NewDestination()
        {
            moveTo = new Vector2(RANDOM.Random.Next((int)Max.X), RANDOM.Random.Next((int)Max.Y));
        }

    }


}
