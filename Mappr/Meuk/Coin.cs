using EngineLib.Core;
using EngineLib.Rendering;
using System.Numerics;

namespace Mappr.Meuk
{
    public class Coin : GameObject
    {
        //LinesRenderer? renderer;
        public Rotator? rotator;

        public override void Awake()
        {
            //renderer = AddComponent<LinesRenderer>();
            //renderer.Points = CreateCircleMesh(10).ToArray();
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
