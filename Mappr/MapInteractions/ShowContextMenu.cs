using Mappr.Controls;
using System.Numerics;

namespace Mappr.MapInteractions
{
    internal class ShowContextMenu : IMapViewInteraction
    {
        private readonly ContextMenuManager<MapMouseEventArgs> contextMenuManager;
        public ShowContextMenu(ContextMenuManager<MapMouseEventArgs> contextMenuManager)
        {
            this.contextMenuManager = contextMenuManager;
        }

        public void HandleMouseClick(object sender, MapMouseEventArgs e)
        {
            if(e.MouseButton == MouseButtons.Right)
            {
                contextMenuManager.ShowMenu(e.MouseScreenPosition, e);
            }
        }

        public void HandleMouseWheel(object sender, MapMouseEventArgs e)
        {
        }


        public void HandleMouseDown(object sender, MapMouseEventArgs e)
        {
        }

        public void HandleMouseUp(object sender, MapMouseEventArgs e)
        {
        }

        public void HandleMouseMove(object sender, MapMouseEventArgs e)
        {
        }
    }



}
