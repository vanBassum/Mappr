using System.Numerics;

namespace EngineLib.Core
{
    public class Transform
    {
        public Vector2 Position { get; set; } = new Vector2();
        public float Rotation { get; set; }  = 0f;
        public Vector2 Scale { get; set; }   = new Vector2(1f, 1f);



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

        public Transform Inverse()
        {
            Vector2 inverseScale = new Vector2(1 / Scale.X, 1 / Scale.Y);
            float inverseRotation = -Rotation;
            Vector2 inversePosition = -Position;

            return new Transform
            {
                Scale = inverseScale,
                Rotation = inverseRotation,
                Position = inversePosition
            };
        }
    }
}
