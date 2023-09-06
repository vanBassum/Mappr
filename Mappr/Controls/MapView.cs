using Mappr.Extentions;
using Mappr.Tiles;
using System.Numerics;

namespace Mappr.Controls
{
    public partial class MapView : UserControl
    {
        private readonly PictureBox pbTiles;
        private readonly PictureBox pbOverlay;
        private readonly CoordinateScaler2D mapScreenScaler;
        private readonly DrawableRenderer drawableRenderer;
        private readonly TileRenderer tileRenderer;
        private readonly MapViewInteractions interactions;
        private MapEntitySource? mapEntitySource;
        private ITileSource? tileSource;

        public MapEntitySource? MapEntitySource
        {
            get => mapEntitySource; 
            set
            {
                mapEntitySource = value; 
               // interactions.MapEntitySource = value;
            }
        }
        public ITileSource? TileSource
        {
            get => tileSource; 
            set
            {
                tileSource = value;
            }
        }


        public MapView()
        {
            InitializeComponent();

            pbTiles = new PictureBox();
            pbOverlay = new PictureBox();
            mapScreenScaler = new CoordinateScaler2D();
            drawableRenderer = new DrawableRenderer(mapScreenScaler);
            tileRenderer = new TileRenderer(mapScreenScaler);
            interactions = new MapViewInteractions(mapScreenScaler);

            this.Controls.Add(pbTiles);
            pbTiles.Controls.Add(pbOverlay);

            pbTiles.Dock = DockStyle.Fill;
            pbOverlay.Dock = DockStyle.Fill;

            pbTiles.BackColor = Color.Transparent;
            pbOverlay.BackColor = Color.Transparent;

            pbTiles.Paint += (s, e) => DrawTiles(e.Graphics);
            pbOverlay.Paint += (s, e) => DrawOverlay(e.Graphics);

            pbTiles.BringToFront();
            pbOverlay.BringToFront();

            pbOverlay.MouseWheel += (s, e) => interactions.HandleMouseWheel(e);
            pbOverlay.MouseDown += (s, e) => interactions.HandleMouseDown(e);
            pbOverlay.MouseUp += (s, e) => interactions.HandleMouseUp(e);
            pbOverlay.MouseMove += (s, e) => interactions.HandleMouseMove(e);

            mapScreenScaler.Scale = Vector2.One;
            mapScreenScaler.Offset = new Vector2(0, 0);

            interactions.RequestRefresh += (s, e) => Redraw();
        }


        public void Redraw()
        {
            pbTiles.Refresh();
            pbOverlay?.Refresh();
        }

        void DrawTiles(Graphics g)
        {
            if (TileSource == null)
                return;

            tileRenderer.RenderTiles(g, TileSource, this.ClientSize.ToVector2());
        }


        void DrawOverlay(Graphics g)
        {
            if (MapEntitySource == null)
                return;

            drawableRenderer.Render(g, MapEntitySource.GetDrawables(), this.ClientSize.ToVector2());
        }
    }







}
