using EngineLib.Core;
using EngineLib.Statics;
using System.Numerics;

namespace Mappr.Meuk
{
    public class Bobbing : GameScript
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


}
