using Mappr.Controls;
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
        //private readonly PlayerEntity playerEntity = new PlayerEntity(new Vector2(75, 75));
        private readonly ContextMenuManager<MapMouseEventArgs> menuManager;
        //private readonly Calibrator calibrator;
        private readonly MemoryManager memoryManager;
        private readonly CancellationTokenSource cancellationTokenSource;   

        public TarkovManager(TarkovSettings settings, MapView mapView)
        {
            this.mapView = mapView;
            //calibrator = new Calibrator(mapView);
            menuManager = new ContextMenuManager<MapMouseEventArgs>(mapView);
            // menuManager.AddMenuItem("Im here!", e =>
            // {
            //     calibrator.AddPoint(localPlayerWorldPos, e.MouseMapPosition);
            // });


            foreach (var map in settings.Maps)
                menuManager.AddMenuItem("Maps/" + map.Name, (e) => LoadMap(map.Path));


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

            //mapView.Entities.Add(playerEntity);
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

            // Check if map needs loading


            // Update player position


            // Set map center


            // Redraw map



            return true;
        }


        public bool IsAttached() => memoryManager.IsAttached;

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            memoryManager.Dispose();
        }

        public void LoadMap(string path)
        {
            FileTileSource tileFileSource = new FileTileSource(path);
            ScalerTileSource tileScaler = new ScalerTileSource(tileFileSource);
            CachingTileSource tileCashing = new CachingTileSource(tileScaler, 1920 * 1080 * 5 / (128 * 128));
            mapView.TileSource = tileCashing;

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

}


