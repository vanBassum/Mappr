using System.Numerics;

namespace Mappr.Games.Tarkov.Models
{
    public class Transform
    {
        public Transform? Parent { get; set; }
        public Vector3 Position { get; set; }       //3x 32
        public Quaternion Rotation { get; set; }    //4x 32
        public Vector3 Scale { get; set; }          //3x 32    

        public Transform GetRoot()
        {
            Transform t = this;

            while (t.Parent != null)
                t = t.Parent;
            return t;
        }

        public Transform CalculateWorldTransform()
        {
            Transform result = new Transform
            {
                Position = Position,
                Rotation = Rotation,
                Scale = Scale,
            };

            // Traverse the hierarchy
            Transform? current = Parent;
            while (current != null)
            {
                result.Position = Vector3.Transform(result.Position, current.Rotation);
                result.Position *= current.Scale;
                result.Position += current.Position;
                result.Rotation = Quaternion.Multiply(current.Rotation, result.Rotation);
                result.Scale *= current.Scale;
                current = current.Parent;
            }

            return result;
        }
    }
}


