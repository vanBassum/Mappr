using Mappr.Controls;
using Mappr.Entities;
using Mappr.Extentions;
using Mappr.Games.Tarkov.MemoryReaders;
using Mappr.Games.Tarkov.Models;
using Mappr.Kernel;
using Mappr.MapInteractions;
using Mappr.Tiles;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace Mappr.Games.Tarkov
{
    public class TarkovManager : IMapManager
    {
        private const int Interval = 100;   // fps => 10
        private readonly MapView mapView;
        private readonly PlayerEntity playerEntity = new PlayerEntity(new Vector2(75, 75));
        private readonly ContextMenuManager<MapMouseEventArgs> menuManager;
        private readonly Calibrator calibrator;
        private readonly MemoryManager memoryManager;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly TarkovSettings settings;
        private TarkovSettings.Map map;
        Vector2 localPlayerWorldPos;

        public TarkovManager(TarkovSettings settings, MapView mapView)
        {
            this.mapView = mapView;
            this.settings = settings;
            calibrator = new Calibrator(mapView);
            menuManager = new ContextMenuManager<MapMouseEventArgs>(mapView);
            menuManager.AddMenuItem("Im here!", e =>
            {
                calibrator.AddPoint(localPlayerWorldPos, e.MouseMapPosition);
                map?.CalibrationPoints.Add(new TarkovSettings.CalibrationPointx {
                    Local = e.MouseMapPosition,
                    World = localPlayerWorldPos
                });

                GameSettingsLoader loader = new GameSettingsLoader();
                loader.SaveToFile(settings, "config/tarkov.yaml");
            });


            foreach (var map in settings.Maps)
                menuManager.AddMenuItem("Maps/" + map.Name, (e) => LoadMap(map));


            memoryManager = new MemoryManagerBuilder()
                .WithTypeReader(new EFTGameObjectManagerMemReader())
                .WithTypeReader(new EFTLocalGameWorldMemReader())
                .WithTypeReader(new EFTPlayerMemReader())
                .WithTypeReader(new EFTTransformMemReader())
                .Build();

            mapView.ConfigInteractions(c => c
                //.AddInteraction(new EntityDragging(entitySource))
                //.AddInteraction(new EntityHover(entitySource))
                .AddInteraction(new Panning())
                .AddInteraction(new Zooming(a => a.WithMinZoom(1f).WithMaxZoom(64f).WithZoomFactor(2f)))
                .AddInteraction(new ShowContextMenu(menuManager))
                );

            mapView.Entities.Add(playerEntity);
            cancellationTokenSource = new CancellationTokenSource();
            Start(cancellationTokenSource.Token);
        }


        void Start(CancellationToken token = default)
        {
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        bool updateResult = Update();
                        stopwatch.Stop();
                        long updateDuration = stopwatch.ElapsedMilliseconds;
                        int remainingTime = Interval - (int)updateDuration;
                        if (remainingTime > 0)
                            await Task.Delay(remainingTime);
                    }
                    catch (Exception)
                    {
                        // In case of any exception, delay for 1 second before continuing.
                        await Task.Delay(Interval);
                    }
                }
            }, token);
        }


        bool Update()
        {
            EFTGameObjectManager? eft = GetEFTData(memoryManager);
            if (eft == null)
                return false;


            var playerTraversedTransform = eft.GameWorld?.MainPlayer?.RootBone?.CalculateWorldTransform();
            if (playerTraversedTransform == null)
                return false;

            // Update player position
            localPlayerWorldPos = new Vector2(playerTraversedTransform.Position.X, playerTraversedTransform.Position.Z);

            // Set map center
            if (calibrator.WorldToMapScaler == null)
                return true;

            playerEntity.MapPosition = calibrator.WorldToMapScaler.ApplyTransformation(localPlayerWorldPos);
            mapView.InvokeIfRequired(() => { 
                mapView.SetCenter(playerEntity.MapPosition);
                mapView.Redraw();
            });

            return true;
        }


        public bool IsAttached() => memoryManager.IsAttached;

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            memoryManager.Dispose();
        }

        public void LoadMap(TarkovSettings.Map newMap)
        {
            string path = newMap.Path;
            map = newMap;
            FileTileSource tileFileSource = new FileTileSource(path);
            ScalerTileSource tileScaler = new ScalerTileSource(tileFileSource);
            CachingTileSource tileCashing = new CachingTileSource(tileScaler, 1920 * 1080 * 5 / (128 * 128));
            mapView.TileSource = tileCashing;

            foreach(var pt in map.CalibrationPoints)
                calibrator.AddPoint(pt.Local, pt.World);


            mapView.SetCenter(Vector2.Zero);
            mapView.Redraw();

        }

        void Redraw()
        {
            //if (calibrator?.WorldToMapScaler == null)
            //    return;
            //
            //var playerMapPos = calibrator.WorldToMapScaler.ApplyTransformation(localPlayerWorldPos);
            //playerEntity.MapPosition = playerMapPos;
            //playerEntity.Rotation = localPlayerWorldRot;
            //
            //mapView.InvokeIfRequired(() =>
            //{
            //    mapView.SetCenter(playerMapPos);
            //    mapView.Redraw();
            //});
        }

        EFTGameObjectManager? GetEFTData(MemoryManager memoryManager)
        {
            if (!memoryManager.IsAttached)
            {
                if (!memoryManager.AttachToProcess("EscapeFromTarkov"))
                    return null;
            }

            var unityBaseAddress = memoryManager.GetProcessModuleBase("UnityPlayer.dll");
            var gameObjectManagerAddress = memoryManager.ReadAddress(unityBaseAddress + 0x17FFD28);
            var gameObjectManager = memoryManager.Read<EFTGameObjectManager>(gameObjectManagerAddress);
            return gameObjectManager;
        }


    }


    public class CalibrationPoint : MapEntity
    {
        private bool hover = false;
        public Vector2 CalculatedMapPosition { get; set; }
        public Vector2 ClickedWorldPosition { get; set; }
        public Vector2 ClickedMapPosition { get; set; }

        public override void Draw(Graphics g, CoordinateScaler2D scaler, Vector2 screenSize)
        {
            var screenPos = scaler.ApplyTransformation(CalculatedMapPosition);
            bool isObjectOnScreen = screenPos.X >= 0 && screenPos.Y >= 0 && screenPos.X < screenSize.X && screenPos.Y < screenSize.Y;
            if (isObjectOnScreen)
            {
                var clickedScreenPos = scaler.ApplyTransformation(ClickedMapPosition);

                if (hover)
                {
                    DrawCross(g, Pens.Green, screenPos);
                    DrawCross(g, Pens.Green, clickedScreenPos);
                }
                else
                {
                    DrawCross(g, Pens.Red, screenPos);
                    DrawCross(g, Pens.Blue, clickedScreenPos);
                }
            }
        }

        public override void HandleMouseMove(object sender, MapMouseEventArgs e)
        {
            var screenPos = e.MapToScreenScaler.ApplyTransformation(CalculatedMapPosition);
            float distance = Vector2.Distance(e.MouseScreenPosition, screenPos);
            hover = distance < 10f;
            e.RequestRedraw = true;
            e.IsActive = hover;
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
    public class Calibrator
    {
        List<CalibrationPoint> calibrationPoints = new List<CalibrationPoint>();
        private readonly MapView mapView;
        public CoordinateScaler2D? WorldToMapScaler { get; private set; }

        public Calibrator(MapView mapView)
        {
            this.mapView = mapView;
            if (calibrationPoints.Count >= 2)
            {
                var worldPositions = calibrationPoints.Select(c => c.ClickedWorldPosition).ToArray();
                var mapPositions = calibrationPoints.Select(c => c.ClickedMapPosition).ToArray();
                WorldToMapScaler = CoordinateRegression.Fit(worldPositions, mapPositions);
                foreach (var c in calibrationPoints)
                    c.CalculatedMapPosition = WorldToMapScaler.ApplyTransformation(c.ClickedWorldPosition);
            }
        }

        public void AddPoint(Vector2 worldPos, Vector2 mapPos)
        {
            CalibrationPoint calibrationPoint = new CalibrationPoint()
            {
                ClickedMapPosition = mapPos,
                ClickedWorldPosition = worldPos,
                CalculatedMapPosition = mapPos
            };
            calibrationPoints.Add(calibrationPoint);
            mapView.Entities?.Add(calibrationPoint);

            Debug.WriteLine($" world = {worldPos}   map = {mapPos}");

            if (calibrationPoints.Count >= 2)
            {
                var worldPositions = calibrationPoints.Select(c => c.ClickedWorldPosition).ToArray();
                var mapPositions = calibrationPoints.Select(c => c.ClickedMapPosition).ToArray();
                WorldToMapScaler = CoordinateRegression.Fit(worldPositions, mapPositions);
                foreach (var c in calibrationPoints)
                    c.CalculatedMapPosition = WorldToMapScaler.ApplyTransformation(c.ClickedWorldPosition);
            }


        }
    }

}


