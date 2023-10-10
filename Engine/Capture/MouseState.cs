using System.Numerics;

namespace EngineLib.Capture
{
    public readonly record struct MouseState
    {
        public required MouseButtons Buttons { get; init; }
        public required Vector2 Position { get; init; }
    }
}
