using EngineLib.Core;
using EngineLib.Statics;

namespace Mappr.Meuk
{
    public class Rotator : GameScript
    {
        public float speed = 1;
        public override void Update()
        {
            GameObject.Transform.Rotation += speed * Time.DeltaTime;
        }
    }


}
