using Mappr.Controls;
using Mappr.Extentions;
using Mappr.Tiles;
using System.Diagnostics;
using System.Net;
using System.Numerics;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        FileDownloader downloader = new FileDownloader();
        MapEntitySource entitySource = new MapEntitySource();
        PlayerEntity playerEntity = new PlayerEntity(new Vector2(75, 75));
        public Form1()
        {
            InitializeComponent();

            this.Controls.Add(this.mapView);
            //mapView.Location = new System.Drawing.Point(20, 20);
            //mapView.Size = new System.Drawing.Size(1024, 1024);
            mapView.Dock = DockStyle.Fill;
            mapView.BorderStyle = BorderStyle.FixedSingle;

            FileTileSource fileSource = new FileTileSource("maps/gta5");
            ScalerTileSource scaler = new ScalerTileSource(fileSource);
            CachingTileSource cashing = new CachingTileSource(scaler, (1920*1080) * 5 / (128*128));
            mapView.TileSource = cashing;
            mapView.MapEntitySource = entitySource;

            entitySource.Add(new MapEntity { MapPosition = new Vector2(100, 100) });
            entitySource.Add(new MapEntity { MapPosition = new Vector2(50, 50) });
            entitySource.Add(playerEntity);

            mapView.MouseMove += MapView_MouseMove;
            //Download();
        }

        private void MapView_MouseMove(object? sender, MapMouseEventArgs e)
        {
            var screenPos = e.Scaler.ApplyTransformation(playerEntity.MapPosition);
            playerEntity.Hover = Vector2.Distance(screenPos, e.ScreenPosition) < 5f;
            e.RequestRedraw = true;
        }

        string GetUri(int z, int x, int y)
        {
            return $"https://s.rsg.sc/sc/images/games/GTAV/map/render/{z}/{x}/{y}.jpg";
        }

        string GetFilePath(int z, int x, int y)
        {
            return $"maps/gta5/{z}/{x}x{y}.jpg";
        }

        private void mapView1_Load(object sender, EventArgs e)
        {
            
        }

        async Task Download()
        {
            for (int z = 0; z < 8; z++)
            {
                int max = 1 << z;
                for (int y = 0; y < max; y++)
                {
                    for (int x = 0; x < max; x++)
                    {
                        var uri = GetUri(z, x, y);
                        var file = GetFilePath(z, x, y);
                        await downloader.DownloadFileAsync(uri, file);
                    }
                }
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            mapView.Redraw();
        }
    }

    public class PlayerEntity : MapEntity
    {
        public bool Hover { get; set; }
        public float Rotation { get; set; } // Angle in radians

        public PlayerEntity(Vector2 initialPosition)
        {
            MapPosition = initialPosition;
            Rotation = 0; // Initial rotation (e.g., facing north)
        }

        public override void Draw(Graphics g, CoordinateScaler2D scaler, Vector2 screenSize)
        {
            // Calculate the screen position of the player
            Vector2 screenPosition = scaler.ApplyTransformation(MapPosition);

            // Calculate the endpoints of the arrow
            Vector2 arrowStart = screenPosition;
            Vector2 arrowEnd = screenPosition + new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation)) * 20; // Adjust arrow length as needed

            // Draw the player icon (e.g., a circle) at the player's position
            if(Hover)
                g.FillEllipse(Brushes.Blue, screenPosition.X - 5, screenPosition.Y - 5, 10, 10);
            else
                g.FillEllipse(Brushes.Red, screenPosition.X - 5, screenPosition.Y - 5, 10, 10);

            // Draw the arrow
            g.DrawLine(Pens.Blue, arrowStart.ToPoint(), arrowEnd.ToPoint());
        }
    }



    public class FileDownloader
    {
        private readonly HttpClient httpClient;

        public FileDownloader()
        {
            httpClient = new HttpClient();
        }

        public async Task DownloadFileAsync(string url, string localFilePath)
        {
            try
            {
                // Extract the directory path from the local file path.
                string directoryPath = Path.GetDirectoryName(localFilePath);

                // Check if the directory exists; if not, create it.
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(localFilePath, fileBytes);
                    Console.WriteLine($"File downloaded and saved to {localFilePath}");
                }
                else
                {
                    Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}