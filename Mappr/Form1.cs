using Mappr.Controls;
using Mappr.Entities;
using Mappr.Extentions;
using Mappr.Kernel;
using Mappr.MapInteractions;
using Mappr.Tiles;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mappr
{

    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        MapEntitySource entitySource = new MapEntitySource();
        PlayerEntity playerEntity = new PlayerEntity(new Vector2(75, 75));
        ContextMenuManager<MapMouseEventArgs> menuManager;
        Vector2 localPlayerWorldPos = Vector2.Zero;
        Calibrator calibrator;

        public Form1()
        {
            InitializeComponent();

            this.Controls.Add(this.mapView);
            mapView.Dock = DockStyle.Fill;
            mapView.BorderStyle = BorderStyle.FixedSingle;

            calibrator = new Calibrator(mapView);

            menuManager = new ContextMenuManager<MapMouseEventArgs>(mapView);
            //menuManager.AddMenuItem("Add point", e =>
            //{
            //    //entitySource.Add(new MapEntity { MapPosition = e.MouseMapPosition });
            //    mapView.Redraw();
            //});
            menuManager.AddMenuItem("Im here!", e =>
            {
                calibrator.AddPoint(localPlayerWorldPos, e.MouseMapPosition);
            });


            FileTileSource tileFileSource = new FileTileSource("maps/tarkov/woods");
            ScalerTileSource tileScaler = new ScalerTileSource(tileFileSource);
            CachingTileSource tileCashing = new CachingTileSource(tileScaler, (1920 * 1080) * 5 / (128 * 128));
            mapView.TileSource = tileCashing;
            mapView.ConfigInteractions(c => c
                //.AddInteraction(new EntityDragging(entitySource))
                //.AddInteraction(new EntityHover(entitySource))
                .AddInteraction(new Panning())
                .AddInteraction(new Zooming(a => a.WithMinZoom(1f).WithMaxZoom(64f).WithZoomFactor(2f)))
                .AddInteraction(new ShowContextMenu(menuManager))
                );

            // mapView.Entities.Add(new MapEntity { MapPosition = new Vector2(100, 100) });
            // mapView.Entities.Add(new MapEntity { MapPosition = new Vector2(50, 50) });
            mapView.Entities.Add(playerEntity);

            Start();
        }

        void PositionUpdated()
        {
            this.InvokeIfRequired(() =>
            {
                this.Text = localPlayerWorldPos.ToString("F3");
            });


            if (calibrator?.WorldToMapScaler == null)
                return;

            var playerMapPos = calibrator.WorldToMapScaler.ApplyTransformation(localPlayerWorldPos);
            playerEntity.MapPosition = playerMapPos;
            mapView.InvokeIfRequired(() =>
            {
                mapView.SetCenter(playerMapPos);
                mapView.Redraw();
            });
        }
        public void Start()
        {
            List<IMemoryReader> readers = new List<IMemoryReader> {
                new EFTGameObjectManagerConverter(),
                new EFTLocalGameWorldConverter(),
                new EFTPlayerConverter(),          
                new TransformConverter()
            };

            MemoryManager memoryManager = new MemoryManager(readers);


            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (!ReadMemory(memoryManager))
                        {
                            await Task.Delay(1000);
                        }
                        else
                            PositionUpdated();
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

            nint unityBase = memoryManager.GetProcessModuleBase("UnityPlayer.dll");
            nint gameObjectManager = memoryManager.ReadAddress(unityBase + 0x17FFD28);

            EFTGameObjectManager? world = memoryManager.Read<EFTGameObjectManager>(gameObjectManager);
            var localPlayerPos = world?.GameWorld?.MainPlayer?.Position ?? Vector3.Zero;


            localPlayerWorldPos = new Vector2(localPlayerPos.X, localPlayerPos.X);

            return true;
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

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            };

            options.Converters.Add(new Vector2JsonConverter());
            calibrationPoints = JsonSerializer.Deserialize<List<CalibrationPoint>>(File.ReadAllText("woods.json"), options);
            mapView.Entities?.AddRange(calibrationPoints);
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

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                WriteIndented = true,
            };

            options.Converters.Add(new Vector2JsonConverter());

            File.WriteAllText("woods.json", JsonSerializer.Serialize(calibrationPoints, options));

        }
    }

    public class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            reader.Read();
            var x = reader.GetSingle();
            reader.Read();
            reader.Read();
            var y = reader.GetSingle();
            reader.Read();

            return new Vector2(x, y);
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteEndObject();
        }
    }

    public class EFTGameObjectManager
    {
        public EFTLocalGameWorld? GameWorld { get; set; }
    }

    public class EFTGameObjectManagerConverter : IMemoryReader<EFTGameObjectManager>
    {
        public EFTGameObjectManager Convert(MemoryManager manager, nint gameObjectManager)
        {
            return new EFTGameObjectManager
            {
                GameWorld = manager.Read<EFTLocalGameWorld>(FindGameWorldAddress(manager, gameObjectManager))
            };
        }

        nint FindGameWorldAddress(MemoryManager memoryManager, nint gameObjectManager)
        {
            nint activeNodes = memoryManager.ReadChain(gameObjectManager + 0x28, 0);
            nint lastActiveNode = memoryManager.ReadChain(gameObjectManager + 0x20, 0);

            nint cObject = memoryManager.ReadAddress(activeNodes + 0x8);    //FIrst one is not okay?

            do
            {
                nint obj = memoryManager.ReadAddress(cObject + 0x10);
                nint nameptr = memoryManager.ReadAddress(obj + 0x60);
                string name = memoryManager.Read<string>(nameptr);
            
                if (name == "GameWorld")
                {
                    return obj;
                }
                cObject = memoryManager.ReadAddress(cObject + 0x8);
            }
            while (cObject != lastActiveNode);

            return 0;
        }



    }

    public class EFTLocalGameWorld
    {
        public EFTPlayer? MainPlayer { get; set; }
    }

    public class EFTLocalGameWorldConverter : IMemoryReader<EFTLocalGameWorld>
    {
        public EFTLocalGameWorld Convert(MemoryManager manager, nint gameWorld)
        {
            nint mainPlayerAddr = manager.ReadChain(gameWorld + 0x30, 0x18, 0x28, 0x118);

            return new EFTLocalGameWorld
            {
                MainPlayer = manager.Read<EFTPlayer>(mainPlayerAddr)
            };
        }
    }




    public class EFTPlayerConverter : IMemoryReader<EFTPlayer>
    {
        public EFTPlayer Convert(MemoryManager manager, nint address)
        {
            return new EFTPlayer
            {
                Position = GetPlayerPosition(manager, address)
            };
        }

        private Vector3 GetPlayerPosition(MemoryManager memoryManager, nint playerBase)
        {
            var head_index = 133;
            nint transObj = memoryManager.ReadChain(playerBase + 0xA8, 0x28, 0x28, 0x10, 0x20 + (0x8 * head_index), 0x10);

            var matrix = memoryManager.ReadAddress(transObj + 0x38);
            var index = memoryManager.Read<int>(transObj + 0x40);

            var matrix_list = memoryManager.ReadAddress(matrix + 0x18);
            var matrix_indices = memoryManager.ReadAddress(matrix + 0x20);

            Vector3 result = memoryManager.Read<Vector3>(matrix_list + 0x28 * index);
            int transformIndex = memoryManager.Read<int>(matrix_indices + 4 * index);

            if (matrix_list == 0)
                return Vector3.Zero;

            while (transformIndex >= 0)
            {
                var tMatrix = memoryManager.Read<Transform>(matrix_list + 0x28 * transformIndex);
                if (tMatrix == null)
                    return Vector3.Zero;

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

                transformIndex = memoryManager.Read<int>(matrix_indices + 4 * transformIndex);
            }

            return result;
        }
    }

    public class EFTPlayer
    {
        public Vector3 Position { get; set; }
    }



    public class TransformConverter : IMemoryReader<Transform>
    {
        public Transform Convert(MemoryManager manager, nint address)
        {
            Transform transform = new Transform();
            transform.Position = manager.Read<Vector3>(address);
            transform.Rotation = manager.Read<Quaternion>(address + 12);
            transform.Scale = manager.Read<Vector3>(address + 28);
            return transform;
        }
    }


    public class Transform
    {
        public Vector3 Position { get; set; }       //3x 32
        public Quaternion Rotation { get; set; }    //4x 32
        public Vector3 Scale { get; set; }          //3x 32    
    }
}


