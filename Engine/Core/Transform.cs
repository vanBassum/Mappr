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
            Vector2 transformedPoint = Rotate(localPoint, Rotation);
            transformedPoint += Position;
            transformedPoint *= Scale;
            return transformedPoint;
        }

        public Transform Inverse()
        {
            // Undo scaling
            Vector2 inverseScale = new Vector2(1 / Scale.X, 1 / Scale.Y);

            // Undo rotation
            float inverseRotation = -Rotation;

            // Undo translation
            Vector2 inversePosition = Rotate((-Position) * Scale, inverseRotation);

            return new Transform
            {
                Position = inversePosition,
                Rotation = inverseRotation,
                Scale = inverseScale
            };
        }

        private Vector2 Rotate(Vector2 vector, float angle)
        {
            float cosTheta = MathF.Cos(angle);
            float sinTheta = MathF.Sin(angle);

            float x = vector.X * cosTheta - vector.Y * sinTheta;
            float y = vector.X * sinTheta + vector.Y * cosTheta;

            return new Vector2(x, y);
        }

        //public Vector2 InverseTransformPoint(Vector2 worldPoint)
        //{
        //    // Undo scaling
        //    Vector2 localPoint = worldPoint / Scale;
        //
        //    // Undo translation
        //    localPoint -= Position;
        //
        //    // Undo rotation
        //    float cosTheta = MathF.Cos(-Rotation);
        //    float sinTheta = MathF.Sin(-Rotation);
        //
        //    float x = localPoint.X * cosTheta - localPoint.Y * sinTheta;
        //    float y = localPoint.X * sinTheta + localPoint.Y * cosTheta;
        //
        //    return new Vector2(x, y);
        //}

        
    }
}
