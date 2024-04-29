using Mappr.Controls;
using Microsoft.VisualBasic;
using System;

namespace Mappr.Entities
{
    public class MapEntitySource : IMapViewInteraction
    {
        public List<MapEntity> Entities { get; } = new List<MapEntity>();

        virtual public void HandleMouseClick(object sender, MapMouseEventArgs e) => Do(e, (i, a) => i.HandleMouseClick(this, e));
        virtual public void HandleMouseDown(object sender, MapMouseEventArgs e) => Do(e, (i, a) => i.HandleMouseDown(this, e));
        virtual public void HandleMouseMove(object sender, MapMouseEventArgs e) => Do(e, (i, a) => i.HandleMouseMove(this, e));
        virtual public void HandleMouseUp(object sender, MapMouseEventArgs e) => Do(e, (i, a) => i.HandleMouseUp(this, e));
        virtual public void HandleMouseWheel(object sender, MapMouseEventArgs e) => Do(e, (i, a) => i.HandleMouseWheel(this, e));

        void Do(MapMouseEventArgs args, Action<IMapViewInteraction, MapMouseEventArgs> func)
        {
            foreach (var entity in Entities)
                func(entity, args);
        }
    }
}

