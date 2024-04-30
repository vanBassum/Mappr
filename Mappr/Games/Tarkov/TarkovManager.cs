using Mappr.Controls;
using Mappr.Extentions;
using Mappr.Games.Tarkov.MemoryReaders;
using Mappr.Games.Tarkov.Models;
using Mappr.Kernel;
using Mappr.MapInteractions;
using Mappr.Tiles;
using System.Numerics;

namespace Mappr.Games.Tarkov
{
    public class TarkovManager
    {
        private readonly MapView mapView;
        private readonly PlayerEntity playerEntity = new PlayerEntity(new Vector2(75, 75));
        private readonly ContextMenuManager<MapMouseEventArgs> menuManager;
        private readonly Calibrator calibrator;

        private Vector2 localPlayerWorldPos = Vector2.Zero;
        private Vector2 localPlayerWorldRot = Vector2.Zero;

        public TarkovManager(MapView mapView)
        {
            this.mapView = mapView;

            calibrator = new Calibrator(mapView);

            menuManager = new ContextMenuManager<MapMouseEventArgs>(mapView);

            menuManager.AddMenuItem("Im here!", e =>
            {
                calibrator.AddPoint(localPlayerWorldPos, e.MouseMapPosition);
            });

            mapView.Entities.Add(playerEntity);
            Start();
        }

        public void LoadMap(string path)
        {
            FileTileSource tileFileSource = new FileTileSource(path);
            ScalerTileSource tileScaler = new ScalerTileSource(tileFileSource);
            CachingTileSource tileCashing = new CachingTileSource(tileScaler, 1920 * 1080 * 5 / (128 * 128));
            mapView.TileSource = tileCashing;
            mapView.ConfigInteractions(c => c
                //.AddInteraction(new EntityDragging(entitySource))
                //.AddInteraction(new EntityHover(entitySource))
                .AddInteraction(new Panning())
                .AddInteraction(new Zooming(a => a.WithMinZoom(1f).WithMaxZoom(64f).WithZoomFactor(2f)))
                .AddInteraction(new ShowContextMenu(menuManager))
                );
        }

        void Redraw()
        {
            if (calibrator?.WorldToMapScaler == null)
                return;

            var playerMapPos = calibrator.WorldToMapScaler.ApplyTransformation(localPlayerWorldPos);
            playerEntity.MapPosition = playerMapPos;
            playerEntity.Rotation = localPlayerWorldRot;

            mapView.InvokeIfRequired(() =>
            {
                mapView.SetCenter(playerMapPos);
                mapView.Redraw();
            });
        }

        void Start()
        {
            List<IMemoryReader> readers = new List<IMemoryReader> {
                new EFTGameObjectManagerMemReader(),
                new EFTLocalGameWorldMemReader(),
                new EFTPlayerMemReader(),
                new EFTTransformMemReader()
            };

            MemoryManager memoryManager = new MemoryManager(readers);

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (ReadMemory(memoryManager))
                            Redraw();
                        else
                            await Task.Delay(1000);
                    }
                    catch
                    {
                        await Task.Delay(1000);
                    }
                }
            });
        }

        bool ReadMemory(MemoryManager memoryManager)
        {
            if (!memoryManager.IsAttached)
            {
                if (!memoryManager.AttachToProcess("EscapeFromTarkov"))
                    return false;
            }

            var unityBaseAddress = memoryManager.GetProcessModuleBase("UnityPlayer.dll");
            var gameObjectManagerAddress = memoryManager.ReadAddress(unityBaseAddress + 0x17FFD28);
            var gameObjectManager = memoryManager.Read<EFTGameObjectManager>(gameObjectManagerAddress);

            var playerWorldTransform = gameObjectManager?.GameWorld?.MainPlayer?.Head?.GetRoot();
            if (playerWorldTransform == null)
                return false;

            localPlayerWorldPos = new Vector2(playerWorldTransform.Position.X, playerWorldTransform.Position.Z);
            return true;
        }
    }

}


