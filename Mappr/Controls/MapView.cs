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
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Mappr.Controls
{
    public partial class MapView : UserControl
    {
        private readonly PictureBox pbTiles = new PictureBox();
        private readonly PictureBox pbOverlay = new PictureBox();
        private readonly CoordinateScaler2D MapScreenScaler = new CoordinateScaler2D();
        private readonly MapViewInteractions interactions; // Create an instance of the interactions class

        public ITileSource? TileSource { get; set; }

        public MapView()
        {
            InitializeComponent();
            this.Controls.Add(pbTiles);
            pbTiles.Controls.Add(pbOverlay);

            pbTiles.Dock = DockStyle.Fill;
            pbOverlay.Dock = DockStyle.Fill;

            pbTiles.BackColor = Color.Transparent;
            pbOverlay.BackColor = Color.Transparent;

            pbTiles.Paint += PbTiles_Paint;
            pbOverlay.Paint += (s, e) => DrawOverlay(e.Graphics);

            pbTiles.BringToFront();
            pbOverlay.BringToFront();

            MapScreenScaler.Scale = Vector2.One;
            MapScreenScaler.Offset = new Vector2(0, 0);

            interactions = new MapViewInteractions(MapScreenScaler); // Initialize interactions class
            interactions.RequestRefresh += (s, e) => Redraw();
            AttachMouseHandlers();
            this.DoubleBuffered= false;  
        }

        private void PbTiles_Paint(object? sender, PaintEventArgs e)
        {
            DrawTiles(e.Graphics);
            //using (Graphics graphics = CreateGraphics())
            //{
            //    var graphicsBuffer = BufferedGraphicsManager.Current.Allocate(graphics, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
            //    
            //    Graphics g = graphicsBuffer.Graphics;
            //
            //    DrawTiles(g);
            //
            //    graphicsBuffer.Render(e.Graphics);
            //
            //}


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
            Vector2 mapPos = new Vector2(45, 30);
            Vector2 screenPos = MapScreenScaler.ApplyTransformation(mapPos);

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
