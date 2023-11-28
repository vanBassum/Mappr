using System.Drawing;
using System.Numerics;

namespace EngineLib.Core
{
    public class Transform
    {
        private Matrix3x2 transform = Matrix3x2.Identity;


        public Transform()
        {

        }

        public Transform(Matrix3x2 transformMatrix)
        {
            transform = transformMatrix;
        }

        public Vector2 Position
        {
            get { return new Vector2(transform.M31, transform.M32); }
            set
            {
                transform.M31 = value.X;
                transform.M32 = value.Y;
            }
        }

        public float Rotation
        {
            get { return (float)Math.Atan2(transform.M21, transform.M11); }
            set
            {
                float cos = (float)Math.Cos(value);
                float sin = (float)Math.Sin(value);
                transform.M11 = cos;
                transform.M12 = sin;
                transform.M21 = -sin;
                transform.M22 = cos;
            }
        }

        public Vector2 Scale
        {
            get
            {
                return new Vector2(
                    (float)Math.Sqrt(transform.M11 * transform.M11 + transform.M12 * transform.M12),
                    (float)Math.Sqrt(transform.M21 * transform.M21 + transform.M22 * transform.M22)
                );
            }
            set
            {
                float scaleX = value.X / Scale.X;
                float scaleY = value.Y / Scale.Y;

                transform.M11 *= scaleX;
                transform.M12 *= scaleX;
                transform.M21 *= scaleY;
                transform.M22 *= scaleY;
            }
        }

        public Matrix3x2 GetTransformationMatrix()
        {
            return transform;
        }
    }
}
