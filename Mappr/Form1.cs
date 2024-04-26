using Mappr.Controls;
using Mappr.Download;
using Mappr.Entities;
using Mappr.Extentions;
using Mappr.Kernel;
using Mappr.MapInteractions;
using Mappr.Tiles;
using System.Numerics;
using System.Threading.Tasks.Dataflow;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        MapEntitySource entitySource = new MapEntitySource();
        PlayerEntity playerEntity = new PlayerEntity(new Vector2(75, 75));
        ContextMenuManager<MapMouseEventArgs> menuManager;

        MemoryManager memoryManager = new MemoryManager();

        Vector2 playerWorldPos;

        List<Vector2> worldPoints = new List<Vector2>();
        List<Vector2> mapPoints = new List<Vector2>();

        CoordinateScaler2D? scaler2;



        public Form1()
        {
            InitializeComponent();

            this.Controls.Add(this.mapView);
            mapView.Dock = DockStyle.Fill;
            mapView.BorderStyle = BorderStyle.FixedSingle;

            menuManager = new ContextMenuManager<MapMouseEventArgs>(mapView);
            menuManager.AddMenuItem("Add point", e =>
            {
                entitySource.Add(new MapEntity { MapPosition = e.MouseMapPosition });
                mapView.Redraw();
            });
            menuManager.AddMenuItem("Im here!", e =>
            {
                worldPoints.Add(playerWorldPos);
                mapPoints.Add(e.MouseMapPosition);

                if (worldPoints.Count >= 2)
                    scaler2 = CoordinateRegression.Fit(worldPoints.ToArray(), mapPoints.ToArray());
            });



            FileTileSource fileSource = new FileTileSource("maps/tarkov/interchange");
            ScalerTileSource scaler = new ScalerTileSource(fileSource);
            CachingTileSource cashing = new CachingTileSource(scaler, (1920 * 1080) * 5 / (128 * 128));
            mapView.TileSource = cashing;
            mapView.MapEntitySource = entitySource;
            mapView.ConfigInteractions(c => c
                .AddInteraction(new EntityDragging(entitySource))
                .AddInteraction(new EntityHover(entitySource))
                .AddInteraction(new Panning())
                .AddInteraction(new Zooming(a => a.WithMinZoom(1f).WithMaxZoom(64f).WithZoomFactor(2f)))
                .AddInteraction(new ShowContextMenu(menuManager))
                );

            entitySource.Add(new MapEntity { MapPosition = new Vector2(100, 100) });
            entitySource.Add(new MapEntity { MapPosition = new Vector2(50, 50) });
            entitySource.Add(playerEntity);

            MapDownloader downloader = new MapDownloader
            {
                GetUriCallback = (z, x, y) => $"https://images.gamemaps.co.uk/mapTiles/tarkov/the_labs_clean_2d_monkimonkimonk/{z}/{x}/{y}.png",
                GetFileCallback = (z, x, y) => $"maps/tarkov/labs/{z}/{x}x{y}.png"
            };

            downloader.Download(7, progress: new Progress<float>(p => this.Text = $"{p:P2}"));
           // MakeImage i = new ();



            worldPoints.AddRange(new Vector2[] {
                new Vector2(3259.0862f, 5148.1704f),
                 new Vector2(-1660.7303f, -1029.1011f),
            });

            mapPoints.AddRange(new Vector2[] {
                new Vector2(105.21094f, 46.25f),
                 new Vector2(35.265625f, 134.11719f),
            });
            scaler2 = CoordinateRegression.Fit(worldPoints.ToArray(), mapPoints.ToArray());
            Start();
        }


        public void Start()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (!memoryManager.IsAttached())
                    {
                        if (!memoryManager.attach("GTA5"))
                            return;
                    }

                    IntPtr xAddr = memoryManager.GetProcessBase() + 0x1D9F800;
                    IntPtr yAddr = memoryManager.GetProcessBase() + 0x1D9F804;

                    playerWorldPos = new Vector2(
                        memoryManager.Read_Float(xAddr),
                        memoryManager.Read_Float(yAddr));

                    if (scaler2 == null)
                        return;
                    var playerMapPos = scaler2.ApplyTransformation(playerWorldPos);
                    playerEntity.MapPosition = playerMapPos;
                    mapView.InvokeIfRequired(() =>
                    {
                        mapView.SetCenter(playerMapPos);
                        mapView.Redraw();
                    });
                }
            });
        }
    }


    class MakeImage
    {
        // Method to get the file path of a tile based on its coordinates
        string GetFile(int z, int x, int y) => $"maps/tarkov/interchange/{z}/{x}x{y}.png";

        public MakeImage()
        {
            int z = 6; // Zoom level, assuming 6 in this example
            int tileWidth = 256; // Width of each tile in pixels
            int tileHeight = 256; // Height of each tile in pixels
            int tileCountX = 36; // Number of tiles in width
            int tileCountY = 36; // Number of tiles in height (you mentioned 36x36)

            // Calculate the size of the large image
            int largeImageWidth = tileCountX * tileWidth;
            int largeImageHeight = tileCountY * tileHeight;

            // Create a large bitmap to hold the combined image
            using (Bitmap largeImage = new Bitmap(largeImageWidth, largeImageHeight))
            {
                // Create a graphics object from the large bitmap
                using (Graphics g = Graphics.FromImage(largeImage))
                {
                    // Loop through each tile
                    for (int x = 0; x < tileCountX; x++)
                    {
                        for (int y = 0; y < tileCountY; y++)
                        {
                            // Get the file path for the current tile
                            string filePath = GetFile(z, x, y);

                            // Check if the file exists before loading it
                            if (!File.Exists(filePath))
                            {
                                Console.WriteLine($"Tile file not found: {filePath}");
                                continue;
                            }

                            // Load the tile image from the file
                            using (Bitmap tile = new Bitmap(filePath))
                            {
                                // Calculate the position of the tile in the large image
                                int offsetX = x * tileWidth;
                                int offsetY = y * tileHeight;

                                // Draw the tile onto the large image at the calculated position
                                g.DrawImage(tile, offsetX, offsetY, tileWidth, tileHeight);
                            }
                        }
                    }
                }

                // Save the large image to the specified file path
                string saveTo = @"C:\Users\bas\Desktop\map.png";
                largeImage.Save(saveTo, System.Drawing.Imaging.ImageFormat.Png);
                Console.WriteLine($"Combined image saved to {saveTo}");
            }
        }
    }


}
