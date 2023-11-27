using EngineLib.Utils;
using EngineLib.Statics;
using EngineLib.Capture;
using EngineLib.Rendering;
using System.Xml.Serialization;
using System.Diagnostics;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EngineLib.Core
{
    public class Engine
    {
        public event EventHandler<TimeSpan> onFrame;
        private TickTask? task;
        private Scene? scene;
        public Engine()
        {

        }

        public void Load(Scene scene)
        {
            task?.Stop();
            this.scene = scene;
            GameEntity.RootEntity = scene.RootEntity;
            task = new TickTask(Loop);
            task.TickInterval = TimeSpan.FromSeconds(1f / 30f);
        }


        MouseState GetInput()
        {
            MouseState state = MouseState.Invalid;
            var cameras = scene?.GetGameEntities().Where(a => a is Camera);
            if (cameras == null)
                return state;
            foreach (var camera in cameras)
            {
                if(camera is Camera cam)
                {
                    state = cam.GetMouseState();
                    if(state.IsValid)
                        return state;
                }
            }

            return state;
        }


        void Loop(TimeInfo timeInfo)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Time.DeltaTime = timeInfo.DeltaTime;
            Time.TimeSinceStart = timeInfo.TimeSinceStart;
            Input.Mouse = GetInput();

            if (scene?.RootEntity == null)
                return;
            Update(scene.RootEntity);
            stopwatch.Stop();
            onFrame?.Invoke(this, stopwatch.Elapsed);
        }


        void Update(GameEntity go)
        {
            while(scene?.Startables?.TryDequeue(out var startable)??false)
                startable.Start();

            go.Update();

            var monos = go.GetComponents<GameScript>();
            foreach(var mono in monos)
                mono.Update();

            foreach (var child in go.Children)
                Update(child);
        }
    }
}
