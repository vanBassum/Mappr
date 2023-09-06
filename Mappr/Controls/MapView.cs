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



    public class MapEntitySource
    {
        private List<MapEntity> entities = new List<MapEntity>();

        public IEnumerable<IDrawable> GetDrawables() => entities;

        public IEnumerable<MapEntity> GetEntities() => entities;


        public void Add(MapEntity entity) => entities.Add(entity);
    }

    public class MapEntity : IDrawable
    {
        public Vector2 MapPosition { get; set; }

        public void Draw(Graphics g, Vector2 screenPos)
        {
            DrawCross(g, Pens.Red, screenPos);
        }

        void DrawCross(Graphics g, Pen pen, Vector2 screenPos, int crossSize = 10)
        {
            // Calculate the starting and ending points for the cross lines
            Point startPointHorizontal = new Point((int)screenPos.X - crossSize, (int)screenPos.Y);
            Point endPointHorizontal = new Point((int)screenPos.X + crossSize, (int)screenPos.Y);
            Point startPointVertical = new Point((int)screenPos.X, (int)screenPos.Y - crossSize);
            Point endPointVertical = new Point((int)screenPos.X, (int)screenPos.Y + crossSize);

            // Draw the horizontal and vertical lines to create the cross
            g.DrawLine(pen, startPointHorizontal, endPointHorizontal);
            g.DrawLine(pen, startPointVertical, endPointVertical);
        }
    }







}
