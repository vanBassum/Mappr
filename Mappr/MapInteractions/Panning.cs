using Mappr.Controls;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Mappr.MapInteractions
{
    internal class Panning : IMapViewInteraction
    {
        private bool isDragging = false;
        private Vector2 lastMouseLocation;



        public void HandleMouseClick(object sender, MapMouseEventArgs e)
        {

        }

        public void HandleMouseWheel(object sender, MapMouseEventArgs e)
        {

        }


        public void HandleMouseDown(object sender, MapMouseEventArgs e)
        {
            if (e.MouseButton == MouseButtons.Left)
            {
                isDragging = true;
                lastMouseLocation = e.MouseScreenPosition;
                e.IsActive = true;
            }
        }

        public void HandleMouseUp(object sender, MapMouseEventArgs e)
        {
            if (e.MouseButton == MouseButtons.Left)
            {
                isDragging = false;
                e.IsActive = false;
            }
        }

        public void HandleMouseMove(object sender, MapMouseEventArgs e)
        {
            if (isDragging)
            {
                Vector2 delta = e.MouseScreenPosition - lastMouseLocation;
                e.Scaler.Offset += delta;
                lastMouseLocation = e.MouseScreenPosition;
                e.RequestRedraw = true;
                e.IsActive = false;
            }
        }
    }



}
