using EngineLib.Capture;
using EngineLib.Rendering;
using System.Numerics;

namespace EngineLib.Core
{
    public class Camera : GameObject
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
            var screenState= mouseCapturing.GetMouseState();
            return new MouseState 
            { 
                Buttons = screenState.Buttons, 
                WorldPosition = ProjectToWorld(screenState.WorldPosition), 
                IsValid = screenState.IsValid,
            };
        }


        private void PictureBox_Paint(object? sender, PaintEventArgs e)
        {
            var rendererableObjects = GameObject.FindObjectsThatHaveComponentOfType<IRenderer>();

            foreach (var rendererableObject in rendererableObjects)
            {
                var renderers = rendererableObject.GetComponent<IRenderer>();
                foreach (var renderer in renderers)
                {
                    renderer.Render(e.Graphics, this, rendererableObject.Transform);
                }
            }
        }

        public Vector2 ProjectToScreen(Vector2 worldPosition)
        {
            // Apply transformations: translation, rotation, and projection
            Vector2 transformedPoint = worldPosition - Transform.Position;
            transformedPoint /= Transform.Scale;

            // Apply rotation (if any)
            float cosTheta = MathF.Cos(-Transform.Rotation);
            float sinTheta = MathF.Sin(-Transform.Rotation);

            float x = transformedPoint.X * cosTheta - transformedPoint.Y * sinTheta;
            float y = transformedPoint.X * sinTheta + transformedPoint.Y * cosTheta;

            transformedPoint = new Vector2(x, y);

            // Convert to screen coordinates
            return transformedPoint;
        }

        public Vector2 ProjectToWorld(Vector2 screenPosition)
        {
            // Invert the screen coordinates
            Vector2 invertedPoint = screenPosition;

            // Apply rotation (if any)
            float cosTheta = MathF.Cos(Transform.Rotation);
            float sinTheta = MathF.Sin(Transform.Rotation);

            float x = invertedPoint.X * cosTheta - invertedPoint.Y * sinTheta;
            float y = invertedPoint.X * sinTheta + invertedPoint.Y * cosTheta;

            invertedPoint = new Vector2(x, y);

            // Apply transformations: scale and translation
            invertedPoint *= Transform.Scale;

            // Translate back to world coordinates
            Vector2 worldPosition = invertedPoint + Transform.Position;

            return worldPosition;
        }


        //protected override void Update()
        //{
        //    GameObject[] objectsInScene = GameObject.FindObjectsOfType<GameObject>();
        //
        //    foreach (GameObject obj in objectsInScene)
        //    {
        //        IRenderer renderer = obj.GetComponent<IRenderer>();
        //        if (renderer != null)
        //        {
        //            // Draw the object
        //            DrawObject(renderer, obj.Transform);
        //        }
        //    }
        //}

        //private void DrawObject(IRenderer renderer, Transform transform)
        //{
        //    // Assuming some rendering logic using transform information
        //    Console.WriteLine($"Drawing object at position {transform.Position.X}, {transform.Position.Y}...");
        //}
    }

}
