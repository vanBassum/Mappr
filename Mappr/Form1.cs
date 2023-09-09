using Mappr.Controls;
using Mappr.Entities;
using Mappr.Extentions;
using Mappr.Kernel;
using Mappr.MapInteractions;
using Mappr.Tiles;
using System.Numerics;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        MapEntitySource entitySource = new MapEntitySource();
        PlayerEntity playerEntity = new PlayerEntity(new Vector2(75, 75));
        ContextMenuManager<MapMouseEventArgs> menuManager;

        System.Timers.Timer timer = new();
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



            FileTileSource fileSource = new FileTileSource("maps/gta5");
            ScalerTileSource scaler = new ScalerTileSource(fileSource);
            CachingTileSource cashing = new CachingTileSource(scaler, (1920 * 1080) * 5 / (128 * 128));
            mapView.TileSource = cashing;
            mapView.MapEntitySource = entitySource;
            mapView.ConfigInteractions(c => c
                .AddInteraction(new EntityDragging(entitySource))
                .AddInteraction(new EntityHover(entitySource))
                .AddInteraction(new Panning())
                .AddInteraction(new Zooming(a => a.WithMinZoom(1f).WithMaxZoom(128f).WithZoomFactor(2f)))
                .AddInteraction(new ShowContextMenu(menuManager))
                );

            entitySource.Add(new MapEntity { MapPosition = new Vector2(100, 100) });
            entitySource.Add(new MapEntity { MapPosition = new Vector2(50, 50) });
            entitySource.Add(playerEntity);

            

            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
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
    }
}
