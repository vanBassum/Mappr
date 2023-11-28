using System.Numerics;

namespace EngineLib.Core
{
    public struct Viewport
    {
        public Vector2 TopLeft { get; }
        public Vector2 BottomRight { get; }

        public Viewport(Vector2 topLeft, Vector2 bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }
    }

}
