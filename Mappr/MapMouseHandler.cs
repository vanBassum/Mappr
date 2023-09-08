using Mappr.Controls;
using Mappr.Entities;
using Mappr.Misc;
using System;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Mappr
{
    public class MapMouseHandler : IDisposable
    {
        private readonly MapView mapView;
        private MapEntity? selectedEntity; // Track the selected entity for dragging
        private Vector2 lastMousePosition; // Track the last mouse position
        private readonly ContextMenuManager contextMenuManager;

        public MapMouseHandler(MapView mapView, MapEntitySource entitySource)
        {
            this.mapView = mapView;
            this.EntitySource = entitySource;
            contextMenuManager = new ContextMenuManager(mapView);

            contextMenuManager.AddMenuItem("Hello", ()=>{ });

            // Subscribe to the custom mouse events
            mapView.MouseWheel += HandleMouseWheel;
            mapView.MouseDown += HandleMouseDown;
            mapView.MouseUp += HandleMouseUp;
            mapView.MouseMove += HandleMouseMove;
            mapView.MouseClick += HandleMouseClick;
        }



        public MapEntitySource EntitySource { get; }

        private void HandleMouseWheel(object? sender, MapMouseEventArgs e)
        {

        }

        private void HandleMouseDown(object? sender, MapMouseEventArgs e)
        {
            foreach (var entity in EntitySource.GetEntities())
            {
                if (IsMouseWithinEntityBounds(e, entity))
                {
                    selectedEntity = entity;
                    lastMousePosition = e.MouseMapPosition;
                    break;
                }
            }
        }

        private void HandleMouseUp(object? sender, MapMouseEventArgs e)
        {
            selectedEntity = null;
        }

        private void HandleMouseMove(object? sender, MapMouseEventArgs e)
        {
            if (selectedEntity != null)
            {
                Vector2 delta = e.MouseMapPosition - lastMousePosition;
                selectedEntity.MapPosition += delta;
                lastMousePosition = e.MouseMapPosition;
                e.RequestRedraw = true;
                e.BlockMapInteractions = true;
            }
            else
            {
                foreach (var entity in EntitySource.GetEntities())
                {
                    bool collide = IsMouseWithinEntityBounds(e, entity);
                    if (entity.MouseHover != collide)
                    {
                        entity.MouseHover = collide;
                        e.RequestRedraw = true;
                    }
                }
            }
        }

        private bool IsMouseWithinEntityBounds(MapMouseEventArgs e, MapEntity entity)
        {
            float radius = 5f;
            var eScreen = e.Scaler.ApplyTransformation(entity.MapPosition);
            return e.MouseScreenPosition.X >= eScreen.X - radius && e.MouseScreenPosition.X <= eScreen.X + radius
                && e.MouseScreenPosition.Y >= eScreen.Y - radius && e.MouseScreenPosition.Y <= eScreen.Y + radius;
        }

        private void HandleMouseClick(object? sender, MapMouseEventArgs e)
        {
            if(e.MouseButton.HasFlag(MouseButtons.Right))
                contextMenuManager.ShowMenu(e.MouseScreenPosition);
        }

        public void Dispose()
        {
            mapView.MouseWheel -= HandleMouseWheel;
            mapView.MouseDown -= HandleMouseDown;
            mapView.MouseUp -= HandleMouseUp;
            mapView.MouseMove -= HandleMouseMove;
            mapView.MouseClick -= HandleMouseClick;
        }
    }

}
