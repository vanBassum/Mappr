using Mappr.Extentions;
using Mappr.Math;
using Mappr.NumericalMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public ITileSource? TileSource { get; set; } = new BufferedTileSource(new FileTileSource("maps"), 16);
        private ContextMenuStrip menu = new ContextMenuStrip();
        PictureBox pbTiles = new PictureBox();
        PictureBox pbOverlay = new PictureBox();
        LinearRegression linearRegression = new LinearRegression();
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
            pbOverlay.Paint += (s, e) => DrawOverlay(e.Graphics, Offset);

            pbTiles.BringToFront();
            pbOverlay.BringToFront();

            //menu.AddMenuItem("I am here!", () => );

        }

        public void Redraw() {
            pbTiles.Refresh();
            pbOverlay?.Refresh();
        }

        void DrawOverlay(Graphics g, Vector2 offset)
        {
            foreach(var item in linearRegression.Samples)
            {
                g.DrawArc(Pens.Blue, new Rectangle((int)item.ScreenPos.X, (int)item.ScreenPos.Y, 5, 5), 0, 360);
            }
        }

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

    public class Scaler
    {
        private readonly Vector2 a;
        private readonly Vector2 b;

        public Scaler(Vector2 a, Vector2 b)
        {
            this.a = a; 
            this.b = b;
        }

        public Vector2 WorldToScreen(Vector2 x)
        {
            return a * x + b;
        }

        public Vector2 ScreenToWorld(Vector2 y)
        {
            return (y - b) / a;
        }

        public static Scaler FromSamples(Coord[] samples)
        {
            if (samples.Length < 2)
                throw new ArgumentException("At least two samples are required.");

            var worldXVals = samples.Select(a => a.WorldPos.X).ToArray();
            var screenXVals = samples.Select(a => a.ScreenPos.X).ToArray();
            var xCooficients = LinearRegression.PerformLinearRegression(worldXVals, screenXVals);

            var worldYVals = samples.Select(a => a.WorldPos.Y).ToArray();
            var screenYVals = samples.Select(a => a.ScreenPos.Y).ToArray();
            var yCooficients = LinearRegression.PerformLinearRegression(worldYVals, screenYVals);

            return new Scaler(new Vector2(xCooficients.A, yCooficients.A), new Vector2(xCooficients.B, yCooficients.B));
        }
    }


    public class Coord
    {
        public Coord(Vector2 worldPos, Vector2 screenPos)
        {
            WorldPos = worldPos;
            ScreenPos = screenPos;
        }

        public Vector2 WorldPos { get; set; }
        public Vector2 ScreenPos { get; set; }
    }

}

