using Mappr.Controls;
using Mappr.Entities;

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


}
