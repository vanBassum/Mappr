using Mappr.Controls;
using Mappr.Download;
using Mappr.Entities;
using Mappr.Extentions;
using Mappr.Kernel;
using Mappr.MapInteractions;
using Mappr.Tiles;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
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



            FileTileSource fileSource = new FileTileSource("maps/tarkov/woods");
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


            //scaler2 = CoordinateRegression.Fit(worldPoints.ToArray(), mapPoints.ToArray());

            Start();




        }

        void PositionUpdated()
        {
            this.InvokeIfRequired(() => {
                this.Text = playerWorldPos.ToString("F3");
            });


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


        public void Start()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    Vector3? worldPos = GetWorldPos();

                    if(worldPos == null)
                    {
                        await Task.Delay(1000);
                    }

                    playerWorldPos = new Vector2(worldPos.Value.X, worldPos.Value.Y);
                    PositionUpdated();

                }
            });
        }


        Vector3? GetWorldPos()
        {
            if (!memoryManager.IsAttached())
            {
                if (!memoryManager.attach("EscapeFromTarkov"))
                    return null;
            }

            nint unityBase = memoryManager.GetProcessModuleBase("UnityPlayer.dll");
            nint gameObjectManager = memoryManager.Read_Address(unityBase + 0x17FFD28);


            nint LastTaggedNode = memoryManager.Read_Address(gameObjectManager + 0x0);
            nint TaggedNodes = memoryManager.Read_Address(gameObjectManager + 0x8);
            nint LastMainCameraTaggedNode = memoryManager.Read_Address(gameObjectManager + 0x10);
            nint MainCameraTaggedNodes = memoryManager.Read_Address(gameObjectManager + 0x18);
            nint LastActiveNode = memoryManager.Read_Address(gameObjectManager + 0x20);
            nint ActiveNodes = memoryManager.Read_Address(gameObjectManager + 0x28);

            nint activeNodes = memoryManager.Read_Address(ActiveNodes);
            nint lastActiveNode = memoryManager.Read_Address(LastActiveNode);

            nint cObject = activeNodes;
            nint gameWorld = 0;
            //TODO: doesnt include last item!
            while (cObject != lastActiveNode)
            {
                nint obj = memoryManager.Read_Address(cObject + 0x10);
                nint nameptr = memoryManager.Read_Address(obj + 0x60);
                string name = memoryManager.Read_String(nameptr, 128);

                if (name == "GameWorld")
                {
                    gameWorld = obj;
                    break;
                }
                cObject = memoryManager.Read_Address(cObject + 0x8);
            }

            if (gameWorld == 0)
                return null;


            nint c1 = memoryManager.Read_Address(gameWorld + 0x30);
            nint c2 = memoryManager.Read_Address(c1 + 0x18);
            nint c3 = memoryManager.Read_Address(c2 + 0x28);
            nint playerBase = memoryManager.Read_Address(c3 + 0x118);

            var ptr_1 = memoryManager.Read_Address(playerBase + 0xA8);
            var ptr_2 = memoryManager.Read_Address(ptr_1 + 0x28);
            var ptr_3 = memoryManager.Read_Address(ptr_2 + 0x28);
            var ptr_4 = memoryManager.Read_Address(ptr_3 + 0x10);
            var head_index = 133;
            var ptr_5 = memoryManager.Read_Address(ptr_4 + 0x20 + (0x8 * head_index));
            var transObj = memoryManager.Read_Address(ptr_5 + 0x10);

            var matrix = memoryManager.Read_Address(transObj + 0x38);
            var index = memoryManager.Read_Int32(transObj + 0x40);

            var matrix_list = memoryManager.Read_Address(matrix + 0x18);
            var matrix_indices = memoryManager.Read_Address(matrix + 0x20);

            Vector3 result = memoryManager.Read_Vector3(matrix_list + 0x28 * index);
            int transformIndex = memoryManager.Read_Int32(matrix_indices + 4 * index);


            while (transformIndex >= 0)
            {
                var tMatrix = memoryManager.Read_Transform3(matrix_list + 0x28 * transformIndex);

                float rotX = tMatrix.Rotation.X;
                float rotY = tMatrix.Rotation.Y;
                float rotZ = tMatrix.Rotation.Z;
                float rotW = tMatrix.Rotation.W;

                float scaleX = result.X * tMatrix.Scale.X;
                float scaleY = result.Y * tMatrix.Scale.Y;
                float scaleZ = result.Z * tMatrix.Scale.Z;

                var x = tMatrix.Position.X + scaleX +
                    (scaleX * ((rotY * rotY * -2.0f) - (rotZ * rotZ * 2.0f))) +
                    (scaleY * ((rotW * rotZ * -2.0f) - (rotY * rotX * -2.0f))) +
                    (scaleZ * ((rotZ * rotX * 2.0f) - (rotW * rotY * -2.0f)));
                var y = tMatrix.Position.Y + scaleY +
                    (scaleX * ((rotX * rotY * 2.0f) - (rotW * rotZ * -2.0f))) +
                    (scaleY * ((rotZ * rotZ * -2.0f) - (rotX * rotX * 2.0f))) +
                    (scaleZ * ((rotW * rotX * -2.0f) - (rotZ * rotY * -2.0f)));
                var z = tMatrix.Position.Z + scaleZ +
                    (scaleX * ((rotW * rotY * -2.0f) - (rotX * rotZ * -2.0f))) +
                    (scaleY * ((rotY * rotZ * 2.0f) - (rotW * rotX * -2.0f))) +
                    (scaleZ * ((rotX * rotX * -2.0f) - (rotY * rotY * 2.0f)));

                result = new Vector3((float)x, (float)y, (float)z);

                transformIndex = memoryManager.Read_Int32(matrix_indices + 4 * transformIndex);
            }

            return result;

        }
    }



 




    public static class Ext
    {
        public static Vector3 Read_Vector3(this MemoryManager memoryManager, nint address)
        {
            return new Vector3(
                memoryManager.Read_Float(address + 0),
                memoryManager.Read_Float(address + 4),
                memoryManager.Read_Float(address + 8));
        }

        public static Vector4 Read_Vector4(this MemoryManager memoryManager, nint address)
        {
            return new Vector4(
                memoryManager.Read_Float(address + 0),
                memoryManager.Read_Float(address + 4),
                memoryManager.Read_Float(address + 8),
                memoryManager.Read_Float(address + 12));
        }

        public static Quaternion Read_Quaternion(this MemoryManager memoryManager, nint address)
        {
            return new Quaternion(
                memoryManager.Read_Float(address + 12),
                memoryManager.Read_Float(address + 16),
                memoryManager.Read_Float(address + 20),
                memoryManager.Read_Float(address + 24));
        }

        public static Transform3 Read_Transform3(this MemoryManager memoryManager, nint address)
        {
            Transform3 transform = new Transform3();
            transform.Position = memoryManager.Read_Vector3(address);           // 0 4 8
            transform.Rotation = memoryManager.Read_Quaternion(address + 12);   // 12 16 20 24
            transform.Scale = memoryManager.Read_Vector3(address + 28);         // 28 32 36
            return transform;
        }
        
        public static Transform4 Read_Transform4(this MemoryManager memoryManager, nint address)
        {
            Transform4 transform = new Transform4();
            transform.Position = memoryManager.Read_Vector4(address);           // 0 4 8 12
            transform.Rotation = memoryManager.Read_Quaternion(address + 16);   // 16 20 24 28
            transform.Scale = memoryManager.Read_Vector4(address + 32);         // 32 36 40 44
            return transform;
        }
    }


    public class Transform3
    { 
        public Vector3 Position { get; set; }       //3x 32
        public Quaternion Rotation { get; set; }    //4x 32
        public Vector3 Scale { get; set; }          //3x 32    
    }

    public class Transform4
    {
        public Vector4 Position { get; set; }       //3x 32
        public Quaternion Rotation { get; set; }    //4x 32
        public Vector4 Scale { get; set; }          //3x 32    
    }

}
