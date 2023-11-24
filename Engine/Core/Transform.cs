using System.Numerics;

namespace EngineLib.Core
{
    public class Transform
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public Transform()
        {
            Position = new Vector2();
            Rotation = 0f;
            Scale = new Vector2(1f, 1f);
        }

        public Vector2 TransformPoint(Vector2 localPoint)
        {
            // Apply translation, rotation, and scale to transform the local point to world space
            float cosTheta = MathF.Cos(Rotation);
            float sinTheta = MathF.Sin(Rotation);

            float x = localPoint.X * cosTheta - localPoint.Y * sinTheta;
            float y = localPoint.X * sinTheta + localPoint.Y * cosTheta;

            Vector2 transformedPoint = new Vector2(x, y);
            transformedPoint += Position;
            transformedPoint *= Scale;

            return transformedPoint;
        }
    }
}
