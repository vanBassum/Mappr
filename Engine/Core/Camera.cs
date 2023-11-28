using EngineLib.Capture;
using EngineLib.Rendering;
using System.Linq;
using System.Numerics;

namespace EngineLib.Core
{
    public class Camera : IComponent
    {
        public GameEntity Entity { get; set; } = GameEntity.Empty;

        private readonly PictureBox pictureBox;
        public Camera(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            pictureBox.Paint += PictureBox_Paint;
        }

        public void Update()
        {
            pictureBox.Refresh();
        }

        private Matrix3x2 GetPerspective(out GameEntity rootEntity)
        {
            GameEntity? entity = Entity;
            rootEntity = Entity;
            Matrix3x2 result = Matrix3x2.Identity;


            while (entity != null)
            {
                var trans = entity.Transform.GetTransformationMatrix();
                if (Matrix3x2.Invert(trans, out var inv))
                    result *= inv;

                rootEntity = entity;  // Update the rootEntity during traversal
                entity = entity.Parent;
            }

            return result;
        }

        private Matrix3x2 GetPictureBoxOffset()
        {
            // Calculate the offset to center the camera view
            SizeF pictureBoxSize = pictureBox.ClientSize;
            SizeF cameraViewSize = new SizeF(pictureBoxSize.Width / 2, pictureBoxSize.Height / 2);
            Vector2 offset = new Vector2(cameraViewSize.Width, cameraViewSize.Height);
            return Matrix3x2.CreateTranslation(offset);
        }

        private void PictureBox_Paint(object? sender, PaintEventArgs e)
        {
            GameEntity rootEntity;
            Matrix3x2 perspective = GetPerspective(out rootEntity);
            Viewport viewport = CalculateViewport(perspective);
            Render(e.Graphics, perspective, rootEntity, viewport);
        }

        void Render(Graphics graphics, Matrix3x2 transform, GameEntity entity, Viewport viewport)
        {
            transform *= entity.Transform.GetTransformationMatrix();

            
            var renderers = entity.GetComponents<IRenderer>();
            foreach (var renderer in renderers)
                renderer.Render(graphics, transform * GetPictureBoxOffset(), viewport);

            foreach (var child in entity.GetChildren())
                Render(graphics, transform, child, viewport);
        }

        private Viewport CalculateViewport(Matrix3x2 perspective)
        {
            SizeF pictureBoxSize = pictureBox.ClientSize;

            // Get the position of the camera in world coordinates
            Vector2 cameraPosition = Vector2.Transform(Vector2.Zero, perspective);

            // Calculate the half-width and half-height of the camera's view
            float halfWidth = pictureBoxSize.Width / 2;
            float halfHeight = pictureBoxSize.Height / 2;

            // Calculate the top-left and bottom-right corners of the viewport
            Vector2 topLeft = cameraPosition - new Vector2(halfWidth, halfHeight);
            Vector2 bottomRight = cameraPosition + new Vector2(halfWidth, halfHeight);

            return new Viewport(topLeft, bottomRight);
        }
    }

}
