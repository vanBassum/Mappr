using System.Diagnostics;

namespace EngineLib.Utils
{
    public class TickTask
    {
        public TimeSpan TickInterval { get => tickInterval; set { tickInterval = value; SetTimer(value); } }

        private readonly Action<TimeInfo> _action;
        private readonly SemaphoreSlim frameRateSemaphore = new SemaphoreSlim(1);
        private readonly System.Threading.Timer tickRateTimer;
        private readonly Task task;
        private readonly CancellationTokenSource cancellationTokenSource;
        private TimeSpan tickInterval = TimeSpan.FromSeconds(1f / 30f);     //30 ticks per second

        public TickTask(Action<TimeInfo> action)
        {
            _action = action;
            tickRateTimer = new System.Threading.Timer(Tick, null, TimeSpan.Zero, TickInterval);
            cancellationTokenSource = new CancellationTokenSource();
            task = Work(cancellationTokenSource.Token);
        }

        ~TickTask()
        {
            Stop().Wait();
            tickRateTimer.Dispose();
            cancellationTokenSource.Dispose();
            frameRateSemaphore.Dispose();
        }

        public async Task Stop()
        {
            cancellationTokenSource.Cancel();
            await task;
        }

        void SetTimer(TimeSpan interval)
        {
            tickRateTimer.Change(TimeSpan.Zero, interval);
        }

        private void Tick(object? state)
        {
            frameRateSemaphore.Release();
        }

        private async Task Work(CancellationToken token)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            float previous = 0;
            while (!token.IsCancellationRequested)
            {
                await frameRateSemaphore.WaitAsync();
                float timeSinceStart = (float)stopwatch.Elapsed.TotalSeconds;
                float deltaTime = timeSinceStart - previous;

                var timeInfo = new TimeInfo()
                {
                    DeltaTime = deltaTime,
                    TimeSinceStart = timeSinceStart
                };

                _action.Invoke(timeInfo);

                previous = timeSinceStart;
            }
        }
    }
}
