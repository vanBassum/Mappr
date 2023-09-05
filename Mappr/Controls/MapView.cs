using Mappr.Extentions;
using Mappr.Tiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mappr.Controls
{
    public partial class MapView : UserControl
    {
        private readonly PictureBox pbTiles = new PictureBox();
        private readonly PictureBox pbOverlay = new PictureBox();
        private readonly CoordinateScaler2D MapScreenScaler = new CoordinateScaler2D();
        private readonly MapViewInteractions interactions; // Create an instance of the interactions class

        public ITileSetSource? TileSource { get; set; }

        public MapView()
        {
            InitializeComponent();
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

            MapScreenScaler.Scale = Vector2.One * 0.5f;
            MapScreenScaler.Offset = new Vector2(0, 0);

            interactions = new MapViewInteractions(MapScreenScaler); // Initialize interactions class
            interactions.RequestRefresh += (s, e) => Redraw();
            AttachMouseHandlers();
        }

        private void AttachMouseHandlers()
        {
            pbOverlay.MouseWheel += (s, e) => interactions.HandleMouseWheel(e);
            pbOverlay.MouseDown += (s, e) => interactions.HandleMouseDown(e);
            pbOverlay.MouseUp += (s, e) => interactions.HandleMouseUp(e);
            pbOverlay.MouseMove += (s, e) => interactions.HandleMouseMove(e);
        }


        public void Redraw() {
            pbTiles.Refresh();
            pbOverlay?.Refresh();
        }

        void DrawTiles(Graphics g)
        {
            if (TileSource == null)
                return;

            TileRenderer tileRenderer = new TileRenderer(TileSource, MapScreenScaler, this.ClientSize.ToVector2());
            tileRenderer.RenderTiles(g);
        }


        void DrawOverlay(Graphics g)
        {

        }
    }
}
