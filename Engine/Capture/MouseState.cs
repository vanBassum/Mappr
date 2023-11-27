using EngineLib.Core;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace EngineLib.Capture
{
    public readonly record struct MouseState
    {
        public required MouseButtons Buttons { get; init; }
        public required float WheelDelta { get; init; }
        public required Vector2 Position { get; init; }
        public required bool IsValid { get; init; }
        public Camera? Camera { get; init; }

        public static MouseState Invalid => new MouseState 
        { 
            Buttons = MouseButtons.None, 
            WheelDelta = 0,
            IsValid = false, 
            Position = Vector2.Zero
        };
    
    
    
    }
}
