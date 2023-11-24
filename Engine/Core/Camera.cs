using EngineLib.Rendering;
using System.Numerics;

namespace EngineLib.Core
{
    public class Camera : GameObject
    {
        private readonly PictureBox pictureBox;

        public Camera(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            pictureBox.Paint += PictureBox_Paint;
        }

        public override void Update() 
        {
            pictureBox.Refresh();
            base.Update();
        }

        private void PictureBox_Paint(object? sender, PaintEventArgs e)
        {
            V2Graphics g = new V2Graphics(e.Graphics);
            var rendererableObjects = GameObject.FindObjectsThatHaveComponentOfType<IRenderer>();

            foreach (var rendererableObject in rendererableObjects)
            {
                var renderers = rendererableObject.GetComponent<IRenderer>();
                foreach (var renderer in renderers)
                {
                    renderer.Render(g, this, rendererableObject);
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
