using EngineLib.Utils;
using EngineLib.Statics;
using EngineLib.Capture;
using System.Diagnostics;

namespace EngineLib.Core
{
    public class Engine
    {
        public event EventHandler<TimeSpan>? onFrame;
        private readonly TickTask task;
        private readonly GameEntity rootEntity;
        public Engine(GameEntity rootEntity)
        {
            this.rootEntity = rootEntity;
            this.task = new TickTask(Loop);
            this.task.TickInterval = TimeSpan.FromSeconds(1f / 30f);
        }

        //MouseState GetInput()
        //{
        //    MouseState state = MouseState.Invalid;
        //    var cameras = scene?.GetGameEntities().Where(a => a is Camera);
        //    if (cameras == null)
        //        return state;
        //    foreach (var camera in cameras)
        //    {
        //        if(camera is Camera cam)
        //        {
        //            state = cam.GetMouseState();
        //            if(state.IsValid)
        //                return state;
        //        }
        //    }
        //
        //    return state;
        //}


        void Loop(TimeInfo timeInfo)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Time.DeltaTime = timeInfo.DeltaTime;
            Time.TimeSinceStart = timeInfo.TimeSinceStart;
            //Input.Mouse = GetInput();

            //if (scene?.RootEntity == null)
            //    return;
            //Update(scene.RootEntity);

            Update(rootEntity);
            stopwatch.Stop();
            onFrame?.Invoke(this, stopwatch.Elapsed);
        }


        void Update(GameEntity go)
        {
            foreach(var component in go.GetComponents())
                component.Update();

            foreach (var child in go.GetChildren())
                Update(child);
        }
    }
}
