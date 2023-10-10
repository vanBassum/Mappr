namespace EngineLib.Utils
{
    public readonly record struct TimeInfo
    {
        public required float TimeSinceStart { get; init; }
        public required float DeltaTime { get; init; }
    }
}
