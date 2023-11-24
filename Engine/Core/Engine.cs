using EngineLib.Utils;
using EngineLib.Statics;
using EngineLib.Capture;
using EngineLib.Rendering;
using System.Xml.Serialization;

namespace EngineLib.Core
{
    public class Engine
    {
        private TickTask? task;
        private Scene? scene;
        public Engine()
        {

        }

        public void Load(Scene scene)
        {
            task?.Stop();
            this.scene = scene;
            GameObject.RootObject = scene.RootObject;
            task = new TickTask(Loop);
            task.TickInterval = TimeSpan.FromSeconds(1f / 30f);
        }

        void Loop(TimeInfo timeInfo)
        {
            Time.DeltaTime = timeInfo.DeltaTime;
            Time.TimeSinceStart = timeInfo.TimeSinceStart;

            if (scene?.RootObject == null)
                return;
            Update(scene.RootObject);
        }


        void Update(GameObject go)
        {
            go.Update();

            var monos = go.GetComponent<MonoBehaviour>();
            foreach(var mono in monos)
                mono.Update();

            foreach (var child in go.Children)
                Update(child);
        }


    }
}
