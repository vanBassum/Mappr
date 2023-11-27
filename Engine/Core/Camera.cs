using EngineLib.Capture;
using EngineLib.Rendering;
using System.Numerics;

namespace EngineLib.Core
{
    public class Camera : GameEntity
    {
        private readonly PictureBox pictureBox;
        private readonly MouseCapturing mouseCapturing;
        public Vector2 ViewPort { get; private set; }
        public Camera(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            this.mouseCapturing = new MouseCapturing(pictureBox);
            pictureBox.Paint += PictureBox_Paint;
            ViewPort = new Vector2(pictureBox.Width, pictureBox.Height);
            pictureBox.Resize += (s,e) => ViewPort = new Vector2(pictureBox.Width, pictureBox.Height);
        }

        public override void Update() 
        {
            pictureBox.Refresh();
            base.Update();
        }

        public MouseState GetMouseState()
        {
            var screenState = mouseCapturing.GetMouseState();
            return new MouseState 
            { 
                Buttons = screenState.Buttons, 
                WheelDelta = screenState.WheelDelta,
                Position = Transform.TransformPoint(screenState.Position - ViewPort / 2), 
                IsValid = screenState.IsValid,
                Camera = this,
            };
        }


        private void PictureBox_Paint(object? sender, PaintEventArgs e)
        {
            if (Scene == null)
                return;
            RenderEntity(e.Graphics, Scene.RootEntity, new Transform());



            var rendererableObjects = GameEntity.FindObjectsThatHaveComponentOfType<IRenderer>();

            foreach (var rendererableObject in rendererableObjects)
            {
                var renderers = rendererableObject.GetComponents<IRenderer>();
                foreach (var renderer in renderers)
                {
                    renderer.Render(e.Graphics, this, rendererableObject.Transform);
                }
            }
        }


        private void RenderEntity(Graphics g, GameEntity entity, Transform transform)
        {
            var renderers = entity.GetComponents<IRenderer>();
            foreach (var renderer in renderers)
                renderer.Render(g, this, entity.Transform);
            





        }





    }

}
