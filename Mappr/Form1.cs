using Mappr.Controls;
using Mappr.Tiles;
using System.Net;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        ITileSetSource mapTileSource;
        FileDownloader downloader = new FileDownloader();
        public Form1()
        {
            InitializeComponent();
            this.Controls.Add(this.mapView);
            //mapView.Location = new System.Drawing.Point(20, 20);
            //mapView.Size = new System.Drawing.Size(1024, 1024);
            mapView.Dock = DockStyle.Fill;
            mapView.BorderStyle = BorderStyle.FixedSingle;
            mapTileSource = new FileTileSetSource("maps/gta5");
            mapView.TileSource = mapTileSource;
            //Download();
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