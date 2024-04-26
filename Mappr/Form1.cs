using Mappr.Controls;
using Mappr.Download;
using Mappr.Entities;
using Mappr.Extentions;
using Mappr.Kernel;
using Mappr.MapInteractions;
using Mappr.Tiles;
using System;
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
                        if (!memoryManager.attach("EscapeFromTarkov"))
                            return;
                    }

                    nint unityBase                  = memoryManager.GetProcessModuleBase("UnityPlayer.dll");
                    nint gameObjectManager          = memoryManager.Read_Address( unityBase + 0x17FFD28);


                    nint LastTaggedNode             = memoryManager.Read_Address( gameObjectManager + 0x0  );
                    nint TaggedNodes                = memoryManager.Read_Address( gameObjectManager + 0x8  );
                    nint LastMainCameraTaggedNode   = memoryManager.Read_Address( gameObjectManager + 0x10 );
                    nint MainCameraTaggedNodes      = memoryManager.Read_Address( gameObjectManager + 0x18 );
                    nint LastActiveNode             = memoryManager.Read_Address( gameObjectManager + 0x20 );
                    nint ActiveNodes                = memoryManager.Read_Address( gameObjectManager + 0x28 );

                    nint activeNodes                = memoryManager.Read_Address(ActiveNodes);
                    nint lastActiveNode             = memoryManager.Read_Address(LastActiveNode);

                    nint cObject = activeNodes;
                    nint gameWorld = 0;
                    //TODO: doesnt include last item!
                    while (cObject != lastActiveNode)
                    {
                        nint obj = memoryManager.Read_Address(cObject + 0x10);
                        nint nameptr = memoryManager.Read_Address(obj + 0x60);
                        string name = memoryManager.Read_String(nameptr, 128);

                        if(name == "GameWorld")
                        {
                            gameWorld = obj;
                            break;
                        }
                        cObject = memoryManager.Read_Address(cObject + 0x8);
                    }

                    if (gameWorld == 0)
                        return;

                    nint c1 = memoryManager.Read_Address(gameWorld + 0x30);
                    nint c2 = memoryManager.Read_Address(c1 + 0x18);
                    nint c3 = memoryManager.Read_Address(c2 + 0x28);
                    nint mainPlayer = memoryManager.Read_Address(c3 + 0x118);


                    nint weaponPort = memoryManager.Read_Address(mainPlayer + 0x1A0);
                    nint hands = memoryManager.Read_Address(weaponPort + 0x18);
                    nint fireport = memoryManager.Read_Address(hands + 0x88);
                    nint test1 = memoryManager.Read_Address(fireport + 0x10);


                    nint body = memoryManager.Read_Address(mainPlayer + 0xA8);
                    nint playerbones = memoryManager.Read_Address(body + 0x20);
                    nint root = memoryManager.Read_Address(playerbones + 0x80);
                    nint test2 = memoryManager.Read_Address(root + 0x10);





                    nint skele = memoryManager.Read_Address(body + 0x28);
                    nint _values = memoryManager.Read_Address(skele +0x28);
                    nint pbase = memoryManager.Read_Address(_values + 0x10);
                    nint cnt = memoryManager.Read_Address(_values + 0x18);

                    int boneIndex = 0;
                    nint start = memoryManager.Read_Address(pbase + 0x20 + boneIndex * 0x8);
                    nint test3 = memoryManager.Read_Address(start + 0x10);





                    //IntPtr gameWorld = FindObject(activeNodes, lastActiveNode, "GameWorld");
                    //
                    //
                    //
                    //
                    //_localGameWorld = Memory.ReadPtrChain(gameWorld, { 0x30, 0x18, 0x28});


                    return;




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


}
