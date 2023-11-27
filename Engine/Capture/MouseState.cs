using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace EngineLib.Capture
{
    public readonly record struct MouseState
    {
        public required MouseButtons Buttons { get; init; }
        public required Vector2 WorldPosition { get; init; }
        public required bool IsValid { get; init; }

        public static MouseState Invalid => new MouseState { Buttons = MouseButtons.None, IsValid = false, WorldPosition = Vector2.Zero };
    }
}
