using System.Numerics;
using EngineLib.Extentions;

namespace EngineLib.Capture
{
    public class MouseCapturing
    {
        private PictureBox pictureBox;
        private readonly object syncLock = new object();
        private MouseButtons buttons;
        private Vector2 position;

        public MouseCapturing(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            buttons = MouseButtons.None;
            position = Vector2.Zero;
            // Register event handlers for PictureBox
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.MouseMove += PictureBox_MouseMove;
        }

        private void PictureBox_MouseDown(object? sender, MouseEventArgs e)
        {
            lock (syncLock)
            {
                buttons |= e.Button;
                position = e.Location.ToVector();
            }
        }

        private void PictureBox_MouseUp(object? sender, MouseEventArgs e)
        {
            lock (syncLock)
            {
                buttons &= ~e.Button;
                position = e.Location.ToVector();
            }
        }

        private void PictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            lock (syncLock)
            {
                position = e.Location.ToVector();
            }
        }

        public MouseState GetMouseState()
        {
            lock (syncLock)
            {
                Point mousePosition = pictureBox.PointToClient(Control.MousePosition);
                bool isWithinBounds = pictureBox.ClientRectangle.Contains(mousePosition);

                return new MouseState
                {
                    Buttons = buttons,
                    WorldPosition = position,
                    IsValid = isWithinBounds
                };
            }
        }
    }
}
