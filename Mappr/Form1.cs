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

            //downloader.Download(7, progress: new Progress<float>(p => this.Text = $"{p:P2}"));
            //MakeImage i = new ();

            //ImageTiler tiler = new ImageTiler();
            //tiler.ProcessImage(@"C:\Users\bas\Desktop\map2.png", @"C:\Users\bas\Desktop\tiles");



            //worldPoints.AddRange(new Vector2[] {
            //    new Vector2(3259.0862f, 5148.1704f),
            //     new Vector2(-1660.7303f, -1029.1011f),
            //});
            //
            //mapPoints.AddRange(new Vector2[] {
            //    new Vector2(105.21094f, 46.25f),
            //     new Vector2(35.265625f, 134.11719f),
            //});

            worldPoints.AddRange(new Vector2[] {
                new Vector2(0, 0),
                 new Vector2(1, 1),
                 new Vector2(10, 10)
            });

            mapPoints.AddRange(new Vector2[] {
                new Vector2(0, 0),
                 new Vector2(1.5f, 1),
                 new Vector2(10, 10),

            });

            // Define the world and map coordinates
            Vector2[] world = new Vector2[]
            {
                new Vector2(1, 1),
                new Vector2(2, 2),
                new Vector2(3, 3),
                new Vector2(4, 4),
                new Vector2(5, 5)
            };

            Vector2[] map = new Vector2[]
            {
                new Vector2(2, 2),
                new Vector2(4, 4),
                new Vector2(7, 6),
                new Vector2(8, 8),
                new Vector2(10, 10)
            };

            scaler2 = CoordinateRegression.Fit(world, map);
            var errors = CoordinateRegression.Error(world, map, scaler2);

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

    public class ErrorsEntity : IDrawable
    {
        public Vector2[] World { get; set; }
        public Vector2[] Map { get; set; }
        public Vector2[] Errors { get; set; }


        virtual public void Draw(Graphics g, CoordinateScaler2D scaler, Vector2 screenSize)
        {
            var screenPos = scaler.ApplyTransformation(MapPosition);
            bool isObjectOnScreen = screenPos.X >= 0 && screenPos.Y >= 0 && screenPos.X < screenSize.X && screenPos.Y < screenSize.Y;
            if (isObjectOnScreen)
            {
                if (MouseHover)
                    DrawCross(g, Pens.Blue, screenPos);
                else
                    DrawCross(g, Pens.Red, screenPos);
            }

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


        public bool IsMouseWithinEntityBounds(MapMouseEventArgs e)
        {
            float radius = 5f;
            var eScreen = e.Scaler.ApplyTransformation(MapPosition);
            return e.MouseScreenPosition.X >= eScreen.X - radius && e.MouseScreenPosition.X <= eScreen.X + radius
                && e.MouseScreenPosition.Y >= eScreen.Y - radius && e.MouseScreenPosition.Y <= eScreen.Y + radius;
        }
    }


}
