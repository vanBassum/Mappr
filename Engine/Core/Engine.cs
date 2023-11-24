using EngineLib.Utils;
using EngineLib.Statics;
using EngineLib.Capture;
using EngineLib.Rendering;

namespace EngineLib.Core
{
    public class Engine
    {
        private TickTask task;
        private Scene? scene;
        public Engine()
        {

        }

        public void Load(Scene scene)
        {
            task?.Stop();
            this.scene = scene;
            Startup();
            task = new TickTask(Loop);
            task.TickInterval = TimeSpan.FromSeconds(1f / 30f);
        }

        void Loop(TimeInfo timeInfo)
        {
            Time.DeltaTime = timeInfo.DeltaTime;
            Time.TimeSinceStart = timeInfo.TimeSinceStart;

            if (scene == null)
                return;

            foreach(var obj in scene.GetGameObjects())
            {
                obj.Update();
            }
        }
    }
}
