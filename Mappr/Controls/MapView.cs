using Mappr.Entities;
using Mappr.Extentions;
using Mappr.Tiles;
using System.Numerics;

namespace Mappr.Controls
{
    public partial class MapView : UserControl
    {
        public new event EventHandler<MapMouseEventArgs> MouseWheel;
        public new event EventHandler<MapMouseEventArgs> MouseDown;
        public new event EventHandler<MapMouseEventArgs> MouseUp;
        public new event EventHandler<MapMouseEventArgs> MouseMove;
        public new event EventHandler<MapMouseEventArgs> MouseClick;

        private readonly PictureBox pbTiles;
        private readonly PictureBox pbOverlay;
        private readonly CoordinateScaler2D mapScreenScaler;
        private readonly DrawableRenderer drawableRenderer;
        private readonly TileRenderer tileRenderer;
        private readonly MapViewInteractionsManager interactionsManager;
        private readonly MapEntitySource mapEntitySource;
        private ITileSource? tileSource;

        public List<MapEntity> Entities => mapEntitySource.Entities;

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
            interactionsManager = new MapViewInteractionsManager(mapScreenScaler, pbOverlay);
            mapEntitySource = new MapEntitySource();

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

            interactionsManager.MouseWheel += (s, e) => MouseWheel?.Invoke(this, e);
            interactionsManager.MouseDown += (s, e) => MouseDown?.Invoke(this, e);
            interactionsManager.MouseUp += (s, e) => MouseUp?.Invoke(this, e);
            interactionsManager.MouseMove += (s, e) => MouseMove?.Invoke(this, e);
            interactionsManager.MouseClick += (s, e) => MouseClick?.Invoke(this, e);
            

            mapScreenScaler.Scale = Vector2.One * 8;
            mapScreenScaler.Offset = new Vector2(0, 0);

            interactionsManager.RequestRefresh += (s, e) => Redraw();
            interactionsManager.AddInteraction(mapEntitySource);
        }


        public void ConfigInteractions(Action<MapViewInteractionsManager> config)
        {
            config.Invoke(interactionsManager);
        }

        public void SetCenter(Vector2 mapCoords)
        {
            var screenCoords = mapScreenScaler.ApplyTransformation(mapCoords);
            var screenCenter = ClientSize.ToVector2() / 2;
            mapScreenScaler.Offset += (screenCenter - screenCoords);
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
            drawableRenderer.Render(g, mapEntitySource.Entities, this.ClientSize.ToVector2());
        }
    }







}
