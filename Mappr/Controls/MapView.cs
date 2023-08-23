using Mappr.Extentions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mappr.Controls
{
    public partial class MapView : UserControl
    {
        //Make null to disable grid.
        private Pen? debugPen = new Pen(Color.Red, 1) { DashStyle = DashStyle.Dash, DashPattern = new float[] { 5, 5 } };

        public Vector2 Offset { get; set; } = Vector2.Zero;
        public ITileSource? TileSource { get; set; } = new BufferedTileSource( new FileTileSource("maps"), 16);

        PictureBox pbTiles = new PictureBox();
        PictureBox pbOverlay = new PictureBox();
        public MapView()
        {
            InitializeComponent();
            this.Controls.Add(pbTiles);
            pbTiles.Controls.Add(pbOverlay);

            pbTiles.Dock = DockStyle.Fill;
            pbOverlay.Dock = DockStyle.Fill;

            pbTiles.BackColor = Color.Transparent;
            pbOverlay.BackColor = Color.Transparent;

            pbTiles.Paint += (s, e) => DrawTiles(e.Graphics, Offset, TileSource);

            pbTiles.BringToFront();
            pbOverlay.BringToFront();

            pbOverlay.MouseMove += picBox_MouseMove;
            pbOverlay.MouseDown += picBox_MouseDown;
        }

        private Vector2 lastDown = Vector2.Zero;
        private Vector2 offsetDown = Vector2.Zero;

        private void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            lastDown = e.Location.ToVector2();
            offsetDown = Offset;
        }

        private void picBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
            {
                var mousePos = e.Location.ToVector2();
                Offset = offsetDown + mousePos - lastDown;
                pbTiles.Refresh();
            }  
        }

        public void Redraw() => pbTiles.Refresh();


        void DrawTiles(Graphics g, Vector2 offset, ITileSource tileSource)
        {
            if (TileSource == null)
                return;

            Stopwatch sw = Stopwatch.StartNew();

            int tileSizeX = (int)tileSource.TileSize.X;
            int tileSizeY = (int)tileSource.TileSize.Y;

            int startX = (int)(-offset.X / tileSizeX);
            int startY = (int)(-offset.Y / tileSizeY);

            int endX = (int)Math.Ceiling((g.VisibleClipBounds.Width - offset.X) / tileSizeX);
            int endY = (int)Math.Ceiling((g.VisibleClipBounds.Height - offset.Y) / tileSizeY);

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    Point tilePosition = new Point(x, y);
                    Point drawPosition = new Point(
                        (int)(offset.X + x * tileSizeX),
                        (int)(offset.Y + y * tileSizeY)
                    );

                    Bitmap? tile = tileSource.GetTile(tilePosition);
                    if (tile != null)
                    {
                        g.DrawImage(tile, drawPosition);
                    }

                    if(debugPen != null)
                        g.DrawString($"{x}x{y}", DefaultFont, Brushes.Red, drawPosition);
                }
            }
            sw.Stop();

            if (debugPen != null)
            {

                for (int y = startY; y <= endY; y++)
                {
                    int drawY = (int)(offset.Y + y * tileSizeY);
                    g.DrawLine(debugPen, 0, drawY, (int)g.VisibleClipBounds.Width, drawY);
                }

                for (int x = startX; x <= endX; x++)
                {
                    int drawX = (int)(offset.X + x * tileSizeX);
                    g.DrawLine(debugPen, drawX, 0, drawX, (int)g.VisibleClipBounds.Height);
                }
                var msg = $"Tiles {sw.ElapsedMilliseconds}ms";
                g.DrawString(msg, DefaultFont, Brushes.Red, new Point(0, 0));
            }
        }
    }
}

