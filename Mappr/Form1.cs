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
        PlayerEntity playerEntity = new PlayerEntity(new Vector2(75, 75));
        ContextMenuManager<MapMouseEventArgs> menuManager;
        Vector2 localPlayerWorldPos = Vector2.Zero;
        Vector2 localPlayerWorldRot = Vector2.Zero;
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

        void Redraw()
        {
            this.InvokeIfRequired(() =>
            {
                this.Text = localPlayerWorldPos.ToString("F3");
            });


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
        public void Start()
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

            var playerWorldTransform = gameObjectManager?.GameWorld?.MainPlayer?.Head?.CalculateWorldTransform();
            if (playerWorldTransform == null)
                return false;

            // Calculate the world position of the player
            localPlayerWorldPos = new Vector2(playerWorldTransform.Position.X, playerWorldTransform.Position.Z);

            var rotTransform = gameObjectManager?.GameWorld?.MainPlayer?.Head;
            if (rotTransform == null)
                return false;

            // // Calculate the looking direction
            // Vector3 forward = Vector3.Transform(Vector3.UnitZ, rotTransform.Rotation);
            // Vector2 forward2D = new Vector2(forward.X, forward.Y);
            // localPlayerWorldRot = Vector2.Normalize(forward2D);

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

    public class EFTLocalGameWorld
    {
        public EFTPlayer? MainPlayer { get; set; }
    }

    public class EFTPlayer
    {
        public Transform? Head { get; set; }

    }
    public class Transform
    {
        public Transform? Parent { get; set; }
        public Vector3 Position { get; set; }       //3x 32
        public Quaternion Rotation { get; set; }    //4x 32
        public Vector3 Scale { get; set; }          //3x 32    

        public Transform GetRoot()
        {
            Transform t = this;

            while (t.Parent != null)
                t = t.Parent;
            return t;
        }

        public Transform CalculateWorldTransform()
        {
            Transform result = new Transform
            {
                Position = this.Position,
                Rotation = this.Rotation,
                Scale = this.Scale,
            };

            // Traverse the hierarchy
            Transform? current = this.Parent;
            while (current != null)
            {
                result.Position = Vector3.Transform(result.Position, current.Rotation);
                result.Position *= current.Scale;
                result.Position += current.Position;
                result.Rotation = Quaternion.Multiply(current.Rotation, result.Rotation);
                result.Scale *= current.Scale;
                current = current.Parent;
            }

            return result;
        }
    }



    public class EFTGameObjectManagerMemReader : IMemoryReader<EFTGameObjectManager>
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
            nint cObject = memoryManager.ReadAddress(activeNodes + 0x8);    //0x00 gives problems, so start at 0x08

            do
            {
                nint obj = memoryManager.ReadAddress(cObject + 0x10);
                nint nameptr = memoryManager.ReadAddress(obj + 0x60);
                string? name = memoryManager.Read<string>(nameptr);
            
                if (name == "GameWorld")
                    return obj;

                cObject = memoryManager.ReadAddress(cObject + 0x8);
            }
            while (cObject != lastActiveNode);
            return 0;
        }
    }

    public class EFTLocalGameWorldMemReader : IMemoryReader<EFTLocalGameWorld>
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

    public class EFTPlayerMemReader : IMemoryReader<EFTPlayer>
    {
        public EFTPlayer Convert(MemoryManager memoryManager, nint playerBase)
        {
            var head_index = 133;
            nint transObj = memoryManager.ReadChain(
                playerBase + 0xA8,          // Player body
                0x28,                       // SkeletonRootJoint
                0x28,                       // _values -> System.Collections.Generic.List<Transform>
                0x10,                       // base of list
                0x20 + (0x8 * head_index),  // Index of head bone -> EFT.BifacialTransform (i think)
                0x10);                      // Original : UnityEngine.Transform

            return new EFTPlayer
            {
                Head = memoryManager.Read<Transform>(transObj)
            };
        }
    }

    public class EFTTransformMemReader : IMemoryReader<Transform>
    {
        public Transform? Convert(MemoryManager memoryManager, nint transObj)
        {
            var matrix = memoryManager.ReadAddress(transObj + 0x38);                            // 
            var index = memoryManager.Read<int>(transObj + 0x40);                               // Index of transform
            var matrix_list = memoryManager.ReadAddress(matrix + 0x18);                         // List of transforms
            var matrix_indices = memoryManager.ReadAddress(matrix + 0x20);                      // List of indexes to parent transform

            return GetTransform(memoryManager, matrix_list, matrix_indices, index);
        }
    
        Transform? GetTransform(MemoryManager memoryManager, nint matrix_list, nint matrix_indices, int index)
        {
            if (index == -1)
                return null;

            nint transformBase = matrix_list + 0x28 * index;
            int parentIndex = memoryManager.Read<int>(matrix_indices + 4 * index);
            Transform? parent = GetTransform(memoryManager, matrix_list, matrix_indices, parentIndex);
            return new Transform
            {
                Parent = parent,
                Position = memoryManager.Read<Vector3>(transformBase),
                Rotation = memoryManager.Read<Quaternion>(transformBase + 12),
                Scale = memoryManager.Read<Vector3>(transformBase + 28)
            };
        }
    }

    
}


