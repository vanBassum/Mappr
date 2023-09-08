using Mappr.Controls;
using Mappr.Entities;
using System.Numerics;

namespace Mappr.MapInteractions
{
    internal class EntityHover : IMapViewInteraction
    {
        private readonly MapEntitySource entitySource;
        public EntityHover(MapEntitySource entitySource) { 
            this.entitySource = entitySource;
        }

        public void HandleMouseClick(object sender, MapMouseEventArgs e)
        {
        }

        public void HandleMouseDown(object sender, MapMouseEventArgs e)
        {
        }

        public void HandleMouseMove(object sender, MapMouseEventArgs e)
        {
            foreach (var entity in entitySource.GetEntities())
            {
                bool collide = entity.IsMouseWithinEntityBounds(e);
                if (entity.MouseHover != collide)
                {
                    entity.MouseHover = collide;
                    e.RequestRedraw = true;
                }
            }
        }

        public void HandleMouseUp(object sender, MapMouseEventArgs e)
        {
        }

        public void HandleMouseWheel(object sender, MapMouseEventArgs e)
        {
        }
    }

    internal class EntityDragging : IMapViewInteraction
    {
        private readonly MapEntitySource entitySource;
        private MapEntity? selectedEntity; // Track the selected entity for dragging
        private Vector2 lastMousePosition; // Track the last mouse position

        public EntityDragging(MapEntitySource entitySource)
        {
            this.entitySource = entitySource;
        }

        public void HandleMouseClick(object sender, MapMouseEventArgs e)
        {
        }


        public void HandleMouseDown(object sender, MapMouseEventArgs e)
        {
            foreach (var entity in entitySource.GetEntities())
            {
                if (entity.IsMouseWithinEntityBounds(e))
                {
                    selectedEntity = entity;
                    lastMousePosition = e.MouseMapPosition;
                    e.IsActive = true;  
                    break;
                }
            }
        }

        public void HandleMouseUp(object sender, MapMouseEventArgs e)
        {
            selectedEntity = null;
            e.IsActive = false;
        }

        public void HandleMouseMove(object sender, MapMouseEventArgs e)
        {
            if (selectedEntity != null)
            {
                Vector2 delta = e.MouseMapPosition - lastMousePosition;
                selectedEntity.MapPosition += delta;
                lastMousePosition = e.MouseMapPosition;
                e.RequestRedraw = true;
                e.IsActive = true;
            }
        }

        public void HandleMouseWheel(object sender, MapMouseEventArgs e)
        {
        }
    }


}
