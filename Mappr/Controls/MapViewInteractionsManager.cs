using System.Windows.Forms;

namespace Mappr.Controls
{
    public class MapViewInteractionsManager
    {
        public event EventHandler<MapMouseEventArgs>? MouseWheel;
        public event EventHandler<MapMouseEventArgs>? MouseDown;
        public event EventHandler<MapMouseEventArgs>? MouseUp;
        public event EventHandler<MapMouseEventArgs>? MouseMove;
        public event EventHandler<MapMouseEventArgs>? MouseClick;
        public event EventHandler? RequestRefresh;
        private readonly CoordinateScaler2D mapScreenScaler;
        private IMapViewInteraction? activeInteraction;
        private readonly List<IMapViewInteraction> interactions = new List<IMapViewInteraction>();
        public MapViewInteractionsManager(CoordinateScaler2D scaler, Control control)
        {
            mapScreenScaler = scaler;
            control.MouseWheel  += (s, e) => Do(e, (IMapViewInteraction a, MapMouseEventArgs ev) => a.HandleMouseWheel(this, ev));
            control.MouseDown   += (s, e) => Do(e, (IMapViewInteraction a, MapMouseEventArgs ev) => a.HandleMouseDown (this, ev));
            control.MouseUp     += (s, e) => Do(e, (IMapViewInteraction a, MapMouseEventArgs ev) => a.HandleMouseUp   (this, ev));
            control.MouseMove   += (s, e) => Do(e, (IMapViewInteraction a, MapMouseEventArgs ev) => a.HandleMouseMove (this, ev));
            control.MouseClick  += (s, e) => Do(e, (IMapViewInteraction a, MapMouseEventArgs ev) => a.HandleMouseClick(this, ev));
        }


        public MapViewInteractionsManager AddInteraction(IMapViewInteraction interaction)
        {
            interactions.Add(interaction);   
            return this;
        }

        void Do(MouseEventArgs e, Action<IMapViewInteraction, MapMouseEventArgs> func)
        {
            MapMouseEventArgs args = new MapMouseEventArgs(e, mapScreenScaler);

            if (activeInteraction != null)
                func(activeInteraction, args);
            else
            {
                foreach(var action in interactions)
                {
                    func(action, args);
                    if (args.IsActive)
                    {
                        activeInteraction = action;
                        break;
                    }
                }
            }
            if(!args.IsActive)
                activeInteraction = null;
            if (args.RequestRedraw)
                RequestRefresh?.Invoke(this, EventArgs.Empty);
        }
    }
}
