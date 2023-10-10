using EngineLib.Utils;
using EngineLib.Statics;
using EngineLib.Capture;
using EngineLib.Rendering;

namespace EngineLib.Core
{
    public class Engine
    {
        private readonly PictureBox pictureBox;
        private readonly TickTask task;
        private readonly MouseCapturing mouseCapturing;
        private readonly List<GameObject> gameObjects;
        public Engine(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            gameObjects = new List<GameObject>();
            mouseCapturing = new MouseCapturing(pictureBox);
            pictureBox.Paint += (s, e) => Render(e.Graphics);

            task = new TickTask(Loop);
            task.TickInterval = TimeSpan.FromSeconds(1f / 30f);  
        }

        public void Register(GameObject gameObject)
        {
            gameObjects.Add(gameObject);    
        }

        void Loop(TimeInfo timeInfo)
        {
            Time.DeltaTime = timeInfo.DeltaTime;
            Time.TimeSinceStart = timeInfo.TimeSinceStart;
            Input.Mouse = mouseCapturing.GetMouseState();

            Update();
            pictureBox.Refresh();
        }


        private void Update()
        {
            foreach (GameObject gameObject in gameObjects)
                gameObject.Update();    
        }

        private void Render(Graphics g)
        {
            V2Graphics graphics = new V2Graphics(g);
            var renderers = gameObjects.SelectMany(go => go.GetComponents<IRenderer>());
            foreach (var renderer in renderers)
            {
                renderer.Render(graphics);
            }
        }
    }
}
