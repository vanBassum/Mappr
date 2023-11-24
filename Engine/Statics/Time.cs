using EngineLib.Core;
using System.Dynamic;

namespace EngineLib.Statics
{
    public class Time
    {
        public static float TimeSinceStart { get; internal set; }
        public static float DeltaTime { get; internal set; }
    }
}
