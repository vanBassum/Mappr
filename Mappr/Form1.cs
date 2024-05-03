using Mappr.Controls;
using Mappr.Entities;
using Mappr.Games.Tarkov;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Core.Events;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Utilities;
using System.Globalization;
using Mappr.Kernel;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        List<GameSettings> Settings = new List<GameSettings>();
        IMapManager? manager;
        System.Windows.Forms.Timer gameManagerTimer;

        public Form1()
        {
            InitializeComponent();

            this.Controls.Add(this.mapView);
            mapView.Dock = DockStyle.Fill;
            mapView.BorderStyle = BorderStyle.FixedSingle;

            GameSettingsLoader loader = new GameSettingsLoader();
            Directory.CreateDirectory("config");
            foreach(var file in Directory.GetFiles("config"))
            {
                var settings = loader.LoadFile(file);
                if(settings != null)
                    Settings.Add(settings);
            }

            TryLoadGameManager();
            gameManagerTimer = new ();
            gameManagerTimer.Interval = 5000;
            gameManagerTimer.Tick += (sender, e) => TryLoadGameManager();
            gameManagerTimer.Start();
        }

        void TryLoadGameManager()
        {
            if(manager == null)
            {
                var settings = Settings.FirstOrDefault(s => MemoryManager.IsProcessRunning(s.Process));   
                if(settings != null)
                    manager = CreateManager(settings);
            }
            else
            {
                if(!manager.IsAttached())
                {
                    //Delete manager
                    manager.Dispose();
                    manager = null;
                }
            }
        }


        IMapManager? CreateManager(GameSettings settings)
        {
            switch (settings.Engine, settings)
            {
                case ("Tarkov", TarkovSettings tarkovSettings):
                    return new TarkovManager(tarkovSettings, mapView);
            }

            return null;
        }
    }



    public interface IMapManager : IDisposable
    {
        bool IsAttached();
    }


    public class GameSettingsLoader
    {
        private readonly IDeserializer deserializer;
        private readonly ISerializer serializer;

        public GameSettingsLoader()
        {
            deserializer = new DeserializerBuilder()
                .WithTypeConverter(new Vector2YamlConverter())
                .IgnoreUnmatchedProperties()
                .Build();

            serializer = new SerializerBuilder()
                .WithTypeConverter(new Vector2YamlConverter())
                .Build();
        }

        public GameSettings? LoadFile(string file)
        {
            GameSettings baseSettings = GetSettings<GameSettings>(file);
            switch (baseSettings.Engine)
            {
                case "Tarkov":
                    return GetSettings<TarkovSettings>(file);
                default:
                    Console.WriteLine("Unsupported engine type.");
                    break;
            }
            return null;
        }

        public void SaveToFile(GameSettings settings, string file)
        {
            using var writer = new StreamWriter(file);
            serializer.Serialize(writer, settings);
        }

        T GetSettings<T>(string file)
        {
            using var reader = new StreamReader(file);
            return deserializer.Deserialize<T>(reader);
        }
    }


    public class Vector2YamlConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(Vector2);

        public object? ReadYaml(IParser parser, Type type)
        {
            if (parser.TryConsume<SequenceStart>(out var start))
            {
                float x, y;
                if (!float.TryParse(parser.Consume<Scalar>().Value, CultureInfo.InvariantCulture, out x))
                    throw new InvalidOperationException("Expected a YAML float");

                if (!float.TryParse(parser.Consume<Scalar>().Value, CultureInfo.InvariantCulture, out y))
                    throw new InvalidOperationException("Expected a YAML float");
                parser.MoveNext();
                return new Vector2(x, y);
            }

            throw new InvalidOperationException("Expected a YAML object or array");
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            if (value is Vector2 vector)
            {
                emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Flow));
                emitter.Emit(new Scalar(vector.X.ToString(CultureInfo.InvariantCulture)));
                emitter.Emit(new Scalar(vector.Y.ToString(CultureInfo.InvariantCulture)));
                emitter.Emit(new SequenceEnd());
            }
            else
                throw new InvalidOperationException($"Expected a type of {typeof(Vector2).Name}");
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


