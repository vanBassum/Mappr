using Mappr.Controls;
using Mappr.Extentions;
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
        public Vector2 MapOffset { get; set; } = new Vector2(128, 0);
        public float MapScale { get; set; } = 1.0f;
        public ITileSource? TileSource { get; set; }
        public IMapObjectsSource? MapObjectsSource { get; set; }
        private readonly ContextMenuStrip menu = new ContextMenuStrip();
        private readonly PictureBox pbTiles = new PictureBox();
        private readonly PictureBox pbOverlay = new PictureBox();
        private Vector2 mouseDragStartPos;
        private Vector2 mouseDragStartMapOffset;

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

            pbOverlay.MouseWheel += PbOverlay_MouseWheel;
            pbOverlay.MouseDown += PbOverlay_MouseDown;
            pbOverlay.MouseMove += PbOverlay_MouseMove;
        }

        private void PbOverlay_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Vector2 mouseDelta = (mouseDragStartPos - e.Location.ToVector2());
                MapOffset = mouseDragStartMapOffset + mouseDelta;
                Redraw();
            }
        }

        private void PbOverlay_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDragStartPos = e.Location.ToVector2();
                mouseDragStartMapOffset = MapOffset;
            }
        }


        private void PbOverlay_MouseWheel(object? sender, MouseEventArgs e)
        {
            Vector2 mouseScreenPos = new Vector2(e.X, e.Y);
            Vector2 mouseMapPosBeforeZoom = (mouseScreenPos / MapScale) + MapOffset;
            float scaleFactor = e.Delta > 0 ? 1.1f : 1 / 1.1f;
            float newMapScale = MapScale * scaleFactor;
            newMapScale = Math.Max(1f, Math.Min(128f, newMapScale));
            Vector2 mouseMapPosAfterZoom = (mouseScreenPos / newMapScale) + MapOffset;
            MapOffset += (mouseMapPosBeforeZoom - mouseMapPosAfterZoom);
            MapScale = newMapScale;
            Redraw();
        }



        public void Redraw() {
            pbTiles.Refresh();
            pbOverlay?.Refresh();
        }

        void DrawTiles(Graphics g)
        {
            if (TileSource == null)
                return;

            // Calculate the size of a single tile at the current zoom level
            Vector2 tileSize = TileSource.TileSize * MapScale;

            // Calculate the zoom level based on the logarithm of MapScale to the base 2
            int zoomLevel = (int)Math.Max(0, Math.Log(MapScale, 2));

            // Calculate the width and height of the destination rectangle based on the current zoom level
            Vector2 destSize = tileSize / (1 << zoomLevel);

            // Calculate the number of tiles to display based on the control's size
            int tilesX = (int)Math.Ceiling(pbTiles.ClientSize.Width / destSize.X);
            int tilesY = (int)Math.Ceiling(pbTiles.ClientSize.Height / destSize.Y);

            // Calculate the top-left tile coordinates to start drawing
            Vector2 start = MapOffset / destSize;

            // Calculate the offset within the top-left tile
            Vector2 offset = new Vector2(start.X - (int)start.X, start.Y - (int)start.Y) * destSize;

            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    int tileX = (int)start.X + x;
                    int tileY = (int)start.Y + y;

                    // Fetch the tile image from the TileSource
                    Bitmap? tile = TileSource.GetTile(zoomLevel, tileX, tileY);

                    if (tile != null)
                    {
                        // Calculate the destination vector for drawing the tile
                        Vector2 destination = new Vector2(x, y) * destSize - offset;
                        RectangleF destRect = new RectangleF(destination.X, destination.Y, destSize.X, destSize.Y);

                        // Draw the tile on the map
                        g.DrawImage(tile, destRect);
                    }
                }
            }
        }


        void DrawOverlay(Graphics g)
        {
            if (MapObjectsSource == null)
                return;

            foreach (var mapObject in MapObjectsSource.GetAll())
            {
                // Ensure the map object has a valid MapPosition
                if (mapObject.MapPosition != null)
                {
                    // Convert the map position to screen coordinates
                    Vector2 screenPos = (mapObject.MapPosition - MapOffset) * MapScale;

                    // Perform drawing for each map object
                    mapObject.Draw(g, screenPos);
                }
            }
        }
    }


}
