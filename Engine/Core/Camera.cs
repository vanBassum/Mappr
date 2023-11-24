using EngineLib.Rendering;

namespace EngineLib.Core
{
    class Camera : GameObject
    {
        private readonly PictureBox pictureBox;

        public Camera(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
        }

        protected override void Update()
        {
            GameObject[] objectsInScene = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objectsInScene)
            {
                IRenderer renderer = obj.GetComponent<IRenderer>();
                if (renderer != null)
                {
                    // Draw the object
                    DrawObject(renderer, obj.Transform);
                }
            }
        }

        private void DrawObject(IRenderer renderer, Transform transform)
        {
            // Assuming some rendering logic using transform information
            Console.WriteLine($"Drawing object at position {transform.Position.X}, {transform.Position.Y}...");
        }
    }

}
